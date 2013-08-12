using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using MAT.Core.InputModels;
using MAT.Core.Mailers;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.Gift;
using MAT.Core.Models.SignUp;
using MAT.Core.Models.Subscription;
using MAT.Core.PaymentProcessor;
using MAT.Core.Services;
using MAT.Web.Helpers.Builders;
using MAT.Web.Infrastructure.Common;
using Raven.Client;

namespace MAT.Web.Controllers
{
    [RequireHttps]
    public class GiftController : MATController
    {
        private readonly IDocumentSession _ravenSession;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly ShippingCalculator _shippingCalculator;
        private readonly IUserMailer _mailer;

        public GiftController(IDocumentSession documentSession, IPaymentProcessor paymentProcessor, IUserMailer mailer, ShippingCalculator shippingCalculator)
        {
            _ravenSession = documentSession;
            _paymentProcessor = paymentProcessor;
            _mailer = mailer;
            _shippingCalculator = shippingCalculator;
        }

        [RequireHttps]
        public ActionResult Index()
        {
            return RedirectToAction("contact", "home");
            ConfigureViewBagListsForPurchase();

            var model = new GiftPurchaseModel();
            return View(model);
        }

        private void ConfigureViewBagListsForPurchase()
        {
            ViewBag.Genders = Gender.GetGenderSelectList();
            ViewBag.Provinces = Province.GetProvinces();
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
        }

        [RequireHttps, HttpPost]
        public ActionResult Index(GiftPurchaseModel model)
        {
            if (!ModelState.IsValid)
            {
                ConfigureViewBagListsForPurchase();
                return View("Index", model);
            }

            try
            {
                Logger.Debug(string.Format("Received a Gift Purchase request from: {0}", SerializeData(model)));
                if (GiftUserExists(model.Lines.First().Recipient.Email))
                {
                    Logger.Warn("Could not purchase gift, user already exists.");
                    ModelState.AddModelError("Error", "Could not complete purchase, the user you are sending a gift to already has an account. Please contact support hello@mailatale.ca");
                    ConfigureViewBagListsForPurchase();
                    return View("Index", model);
                }

                var products = RavenSession.Query<SubscriptionProduct>().Where(s => s.Status == ProductStatus.Active).ToDictionary(p => p.Name == "Book Box Plus" ? "plus" : "basic", g => g, StringComparer.OrdinalIgnoreCase);
                var selectedSubscription = products[model.Lines.First().Subscription];

                var purchase = new GiftPurchase(model.Email, model.FullName, model.Lines.First(), selectedSubscription, model.PersonalMessage, DateTime.Parse(model.SendGiftNoticeOn));
                var order = new GiftOrder(purchase, model.BillingAddress.ToAddress(), model.PaymentInfo, _shippingCalculator);
                RavenSession.Store(order);
                RavenSession.Store(purchase);

                var paymentResult = _paymentProcessor.ProcessPayment(order);

                if (paymentResult.Result == PaymentResults.Approved)
                {
                    purchase.DatePurchased = DateTime.Now;
                    purchase.Purchased = true;
                    RavenSession.Store(purchase);
                    RavenSession.SaveChanges();
                    TempData["GiftPurchase"] = purchase;
                    try
                    {
                        if (purchase.SendGiftNoticeOn.Date <= DateTime.Today)
                        {
                            var id = purchase.RedeemId.ToString("N").ToLowerInvariant();
                            var link = Url.Action("Redeem", "Gift", new { id }, "https");
                            _mailer.WelcomeGift(purchase, link, purchase.PersonalMessage);
                            purchase.GiftNoticeSent = true;
                            RavenSession.Store(purchase);
                            RavenSession.SaveChanges();
                        }

                        _mailer.PurchaseGift(purchase.SenderEmail);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to send gift e-mails.", e);
                    }

                    return RedirectToAction("thankyou", "gift");
                }

                ConfigureViewBagListsForPurchase();
                return View("Index", model);
            }
            catch (Exception e)
            {
                Logger.Fatal("Redeem failed", e);
                return RedirectToAction("Index", "Home");
            }
        }

        private bool GiftUserExists(string email)
        {
            var existing = _ravenSession.GetUserByCredentialId(email);
            return existing != null;
        }

        public ActionResult ThankYou()
        {
            try
            {
                var purchase = TempData["GiftPurchase"] as GiftPurchase;
                if (purchase == null)
                {
                    Logger.Warn("Thank You Page for gift purchase but could not get purchase details.");
                    return View();
                }

                Logger.Info("Thank You Page for gift purchase");
                var message = string.Format("The following user just Purchased a gift {0} - {3} months. Name: {1} Email {2}", purchase.SubscriptionProduct.Name, purchase.SenderName, purchase.SenderEmail, purchase.SubscriptionLength);
                _mailer.InternalGeneral(message);
                return View();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Error displaying ThankYou Gift page.", ex);
            }
            return View();
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
            ViewBag.Years = new SelectList(Enumerable.Range(2003, 10).OrderByDescending(k => k));
        }

        [RequireHttps]
        public ActionResult Redeem(Guid id)
        {
            return RedirectToAction("contact", "home");
            var purchase = RavenSession.Query<GiftPurchase>().Where(p => p.RedeemId == id).FirstOrDefault();
            if (purchase == null || !purchase.Purchased || purchase.Redeemed)
                return RedirectToAction("Index", "Home");

            ConfigureViewBagLists();

            var model = new GiftRecipientModel(purchase.Id);
            model.ReaderInfo.Name = purchase.RecipientReaderName;
            model.ReaderInfo.PreferenceAnswers = PreferenceQuestionAnswerModel.ListFor(ViewBag.PreferenceQuestions as List<PreferenceQuestion>);
            model.AccountInfo.Email = purchase.RecipientEmail;
            return View(model);
        }

        private bool Validate(GiftRecipientModel model)
        {
            if (!ModelState.IsValid)
                return false;
            if (!model.AccountInfo.EmailMatches)
            {
                ModelState.AddModelError("AccountInfo.Email", "E-mails must match.");
                ModelState.AddModelError("AccountInfo.ConfirmEmail", "E-mails must match.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Return the redeem form with submitted values (other than those that should be cleared),
        /// as well as an optional error message.
        /// </summary>
        private ActionResult Error(GiftRecipientModel model, string message = null)
        {
            ConfigureViewBagLists();
            if (message != null)
                TempData["ErrorMessage"] = message;
            if (model == null)
                model = new GiftRecipientModel();
            if (model.AccountInfo == null)
                model.AccountInfo = new RegisterAccountModel();
            if (model.ReaderInfo == null)
                model.ReaderInfo = new ReaderModel();
            model.AccountInfo.ConfirmEmail = null;
            model.AccountInfo.Password = null;
            return View(model);
        }

        [RequireHttps]
        [HttpPost]
        public ActionResult Redeem(GiftRecipientModel model)
        {
            if (!Validate(model))
                return Error(model);

            var userBuilder = new UserBuilder(RavenSession, Logger);
            var orderBuilder = new OrderBuilder(RavenSession, Logger, new ShippingCalculator(RavenSession));
            try
            {
                Logger.Debug(string.Format("Received a Redeem request from: {0}", SerializeData(model)));
                var purchase = RavenSession.Load<GiftPurchase>(model.GiftPurchaseId);
                if (purchase == null)
                    return RedirectToAction("Index", "Home");
                if (!purchase.Purchased || purchase.Redeemed)
                    return RedirectToAction("Index", "Home");

                User user;
                if (!userBuilder.TryValidateAccountInfo(model.AccountInfo, out user))
                    return Error(model, "This e-mail already exists. Please login instead.");
                UserSubscription subscription;
                if (!orderBuilder.TryBuildSubscription(user, model.ReaderInfo, purchase.SubscriptionProduct, out subscription))
                    return Error(model, "There was a problem creating your subscription.");

                FormsAuthentication.SetAuthCookie(user.UserCredentials.Email, createPersistentCookie: false);
                subscription.SubscriptionStatus = SubscriptionStatus.Active;
                RavenSession.Store(subscription);
                user.Subscriptions.Add(subscription.Id);
                RavenSession.Store(user);
                purchase.RedeemedBy(user, subscription);
                RavenSession.Store(purchase);
                RavenSession.SaveChanges();
                TempData["GiftPurchase"] = purchase;
                return RedirectToAction("thankyou", "signup");
            }
            catch (Exception e)
            {
                Logger.Fatal("Redeem failed", e);
                return Error(model, "Sorry, your sign-up could not be completed at this time. Please let us know so we can help you: hello@mailatale.ca");
            }
        }
    }
}
