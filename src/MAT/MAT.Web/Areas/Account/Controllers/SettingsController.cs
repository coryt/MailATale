using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MAT.Core.InputModels;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Core.Models.Subscription;
using MAT.Core.PaymentProcessor;
using MAT.Web.Controllers;
using MAT.Web.Infrastructure.Common;

namespace MAT.Web.Areas.Account.Controllers
{
    [Authorize]
    public class SettingsController : MATController
    {
        private readonly IPaymentProcessor _paymentProcessor;

        public SettingsController(IPaymentProcessor paymentProcessor)
        {
            _paymentProcessor = paymentProcessor;
        }

        [HttpGet]
        public ActionResult Index()
        {
            User user = RavenSession.GetCurrentUser();
            //List<Reader> readers = RavenSession.Load<Reader>(user.Readers.Select(reader => reader.Id)).ToList();
            ViewBag.NeedsAccountInfo = !user.HasAccountInfo;
            var model = new AccountSettingsModel { Readers = new List<Reader>() };
            return View(model);
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult ChangeAccount()
        {
            User user = RavenSession.GetCurrentUser();
            var model = new ChangeAccountModel(user);

            return PartialView("_ChangeAccount", model);
        }

        [HttpPost]
        public ActionResult ChangeAccount(ChangeAccountModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    HttpContext.Response.StatusCode = 500;
                    HttpContext.Response.Clear();
                    return PartialView("_ChangeAccount", model);
                }

                User user = RavenSession.GetCurrentUser();
                user.Addresses.Clear();
                user.AddAddress(model.ShippingAddress.ToAddress(), model.UseShippingAsBilling);
                if (!model.UseShippingAsBilling)
                {
                    user.AddAddress(model.BillingAddress.ToAddress());
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                RavenSession.Store(user);

                if (user.PaymentProviderId == null)
                    return PartialView("_ChangeAccountSuccess");

                _paymentProcessor.ModifyAccount(user, user.BillingAddress, user.ShippingAddress, null);
                return PartialView("_ChangeAccountSuccess");
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating account settings: " + model.Email, ex);
                ModelState.AddModelError("", "Error updating account.");
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.Clear();
                return PartialView("_ChangeAccount", model);
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            User user = RavenSession.GetCurrentUser();

            try
            {
                if (!ModelState.IsValid)
                {
                    HttpContext.Response.StatusCode = 500;
                    HttpContext.Response.Clear();
                    return PartialView("_ChangePassword", model);
                }

                if (user.UserCredentials.ValidatePassword(model.OldPassword))
                {
                    user.UserCredentials.SetPassword(model.NewPassword);
                    RavenSession.Store(user);
                    return PartialView("_ChangePasswordSuccess");
                }

                ModelState.AddModelError("OldPassword", "Old password did not match existing password");
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.Clear();
                return PartialView("_ChangePassword", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Error attempting to change password: " + user.Email, ex);
                ModelState.AddModelError("OldPassword", "Error changing password.");
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.Clear();
                return PartialView("_ChangePassword", model);
            }
        }
    }
}