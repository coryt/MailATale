using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using MAT.Core.InputModels;
using MAT.Core.Mailers;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.Gift;
using MAT.Core.Models.SignUp;
using MAT.Core.Models.Subscription;
using MAT.Core.PaymentProcessor;
using MAT.Core.Services;
using MAT.Web.Helpers.Builders;
using MAT.Web.Infrastructure.Common;

namespace MAT.Web.Controllers
{
    public class SignUpController : MATController
    {
        private readonly IUserMailer _mailer;
        private readonly IPaymentProcessor _paymentProcessor;

        public SignUpController(IUserMailer mailer, IPaymentProcessor paymentProcessor)
        {
            _mailer = mailer;
            _paymentProcessor = paymentProcessor;
        }

        [AllowAnonymous, RequireHttps]
        public ActionResult Index()
        {
            return RedirectToAction("contact", "home"); 
            ConfigureViewBagLists();

            var model = new SignUpModel()
            {
                Lines = {
                    new SignUpLineItem(),
                    new SignUpLineItem(),
                    new SignUpLineItem()
                }
            };
            model.Lines.ForEach(li => li.Reader.PreferenceAnswers = PreferenceQuestionAnswerModel.ListFor(ViewBag.PreferenceQuestions as List<PreferenceQuestion>));
            return View(model);
        }

        private void ConfigureViewBagLists()
        {
            ViewBag.Relationships = Relationships.GetRelationshipList();
            ViewBag.Genders = Gender.GetGenderSelectList();
            ViewBag.GradeLevels = GradeLevel.GetGradeLevels();
            ViewBag.ReadingLevels = ReadingLevel.GetReadingLevels();
            ViewBag.PreferenceQuestions = RavenSession.GetSignupPreferenceQuestions();
            ViewBag.Provinces = Province.GetProvinces();
            ViewBag.Days = new SelectList(Enumerable.Range(1, 31));
            ViewBag.Months = new SelectList(Enumerable.Range(1, 12));
            ViewBag.Years = new SelectList(Enumerable.Range(2004, 10).OrderByDescending(k => k));
            ViewBag.Genders = Gender.GetGenderSelectList();
            ViewBag.ExpiryMonths = new SelectList(new[]
                {
                    new SelectListItem {Text = "01 - Jan", Value = "01"},
                    new SelectListItem {Text = "02 - Feb", Value = "02"},
                    new SelectListItem {Text = "03 - Mar", Value = "03"},
                    new SelectListItem {Text = "04 - Apr", Value = "04"},
                    new SelectListItem {Text = "05 - May", Value = "05"},
                    new SelectListItem {Text = "06 - Jun", Value = "06"},
                    new SelectListItem {Text = "07 - Jul", Value = "07"},
                    new SelectListItem {Text = "08 - Aug", Value = "08"},
                    new SelectListItem {Text = "09 - Sept", Value = "09"},
                    new SelectListItem {Text = "10 - Oct", Value = "10"},
                    new SelectListItem {Text = "11 - Nov", Value = "11"},
                    new SelectListItem {Text = "12 - Dec", Value = "12"}
                }, "Value", "Text");
            ViewBag.ExpiryYears = new SelectList(Enumerable.Range(DateTime.Now.Year, 6).Select(r => new SelectListItem() { Text = r.ToString(), Value = r.ToString() }), "Value", "Text");
            ViewBag.Packages = RavenSession.Query<SubscriptionProduct>().Where(s => s.Status == ProductStatus.Active).ToList();
        }

        private bool Validate(SignUpModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return false;

                if (!model.Account.EmailMatches)
                {
                    ModelState.AddModelError("Account.Email", "E-mails must match.");
                    ModelState.AddModelError("Account.ConfirmEmail", "E-mails must match.");
                    return false;
                }
                return true;
            }
            finally
            {
                if (model != null && model.Account != null)
                {
                    model.Account.ConfirmEmail = null;
                }
            }
        }

        [HttpPost, RequireHttps]
        public ActionResult Index(SignUpModel model)
        {
            if (!Validate(model))
            {
                ConfigureViewBagLists();
                return View(model);
            }

            Logger.Debug(string.Format("Received a SignUp request from: {0}", SerializeData(model)));

            var userBuilder = new UserBuilder(RavenSession, Logger);
            var orderBuilder = new OrderBuilder(RavenSession, Logger, new ShippingCalculator(RavenSession));
            try
            {
                User user;
                if (!userBuilder.TryValidateAccountInfo(model.Account, out user))
                {
                    ModelState.AddModelError("Account.Email", "This email already exists. Please login instead");
                    ConfigureViewBagLists();
                    return View(model);
                } 

                Promotion promotion;
                if (!TryValidatePromotion(model.ReferralName, out promotion))
                {
                    ModelState.AddModelError("ReferralName", "Invalid referral code. Please enter a valid referral code");
                    ConfigureViewBagLists();
                    return View(model);
                }

                SubscriptionOrder subscriptionOrder;
                if (!orderBuilder.TryBuildOrder(model, user, promotion, out subscriptionOrder))
                {
                    ModelState.AddModelError("Error", "There was a problem with your order, please verify payment information");
                    ConfigureViewBagLists();
                    return View(model); 
                }

                string errorMessage;
                if (!TryPlaceOrder(subscriptionOrder, out errorMessage))
                {
                    ModelState.AddModelError("Error", "There was an error processing your order, please try again");
                    ConfigureViewBagLists();
                    return View(model); 
                }

                FormsAuthentication.SetAuthCookie(user.UserCredentials.Email, createPersistentCookie: false);

                user.PaymentProviderId = subscriptionOrder.TransactionResponse.AccountId;
                subscriptionOrder.Subscriptions.ForEach(us =>
                {
                    RavenSession.Store(us);
                    user.Subscriptions.Add(us.Id);
                });
                RavenSession.Store(user);
                RavenSession.Store(subscriptionOrder);
                RavenSession.SaveChanges();

                _mailer.Welcome(user, subscriptionOrder);
                return RedirectToAction("thankyou", "signup");
            }
            catch (Exception ex)
            {
                Logger.Fatal("Signup failed", ex);
                ConfigureViewBagLists();
                ModelState.AddModelError("Error", "Sorry, your Signup could not be completed at this time. Please let us know so we can help you. hello@mailatale.ca");
                return View(model);
            }
        }

        private bool TryValidatePromotion(string referral, out Promotion promotion)
        {
            promotion = null;
            if (string.IsNullOrEmpty(referral)) return true;

            promotion = RavenSession.Load<Promotion>(referral);

            return promotion != null;
        }

        private bool TryPlaceOrder(SubscriptionOrder subscriptionOrder, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                subscriptionOrder.ProcessPayment(_paymentProcessor);

                if (subscriptionOrder.OrderStatus != OrderStatuses.Paid)
                {
                    errorMessage = subscriptionOrder.PaymentResult + " " + subscriptionOrder.PaymentMessage;
                    return false;
                }

                subscriptionOrder.CreditCardSecurityCode = string.Empty;
                subscriptionOrder.CreditCardNumber = new string('X', subscriptionOrder.CreditCardNumber.Length - 4) + subscriptionOrder.CreditCardNumber.Substring(subscriptionOrder.CreditCardNumber.Length - 4);
                return true;

            }
            catch (Exception ex)
            {
                Logger.Fatal("Error processing order.", ex);
                return false;
            }
        }

        [Authorize]
        public ActionResult ThankYou()
        {
            string message = "Someone signed up but there was an error getting their info.";
            try
            {
                var purchase = TempData["GiftPurchase"] as GiftPurchase;

                if (purchase != null && purchase.Redeemed)
                {
                    string action = "Redeemed";
                    string name = purchase.RecipientName;
                    string email = purchase.RecipientEmail;
                    Logger.Info("Thank You Page for gift " + action);
                    message = string.Format("The following user just {0} a gift. Name: {1} Email {2}", action, name,
                                            email);
                }
                else
                {
                    var user = RavenSession.GetCurrentUser();
                    Logger.Info("Thank You Page for user: " + user.Email);

                    message = string.Format("The following user just signed up. Name: {0} {1} Email {2}", user.FirstName,
                                            user.LastName, user.Email);
                }
                
                _mailer.InternalGeneral(message);

                return View();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Error displaying ThankYou page. Could not fetch user.", ex);
                _mailer.InternalGeneral(message);
            }

            return View();
        }
    }
}