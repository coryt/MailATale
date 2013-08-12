using System;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using MAT.Core.Extensions;
using MAT.Core.InputModels;
using MAT.Core.Mailers;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Web.Controllers;
using MAT.Web.Infrastructure.Common;
using MAT.Web.ViewModels;

namespace MAT.Web.Areas.Account.Controllers
{
    public class LoginController : MATController
    {
        private readonly IUserMailer _mailer;

        public LoginController(IUserMailer mailer)
        {
            _mailer = mailer;
        }

        [HttpGet]
        public ActionResult Index(string returnUrl, bool? reset)
        {
            return RedirectToAction("contact", "home");
            if (Request.IsAuthenticated)
            {
                return RedirectFromLoginPage();
            }
            var model = new LogOnModel { ReturnUrl = returnUrl };
            if (reset.HasValue && reset.Value)
                model.DisplayMessage = "Reset password successful. Please login with your new password.";
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LogOnModel input)
        {
            return RedirectToAction("contact", "home");
            User user = RavenSession.GetUserByCredentialId(input.Login);

            if (user == null || user.UserCredentials.ValidatePassword(input.Password) == false)
            {
                input.DisplayMessage = "Email and password do not match to any known user.";
                ModelState.AddModelError("UserNotExistOrPasswordNotMatch",
                                         "Email and password do not match to any known user.");
                return View(input);
            }

            if (ModelState.IsValid)
            {
                FormsAuthentication.SetAuthCookie(input.Login, true);
                return RedirectFromLoginPage(input.ReturnUrl);
            }

            return View(new LogOnModel { Login = input.Login, ReturnUrl = input.ReturnUrl });
        }

        private ActionResult RedirectFromLoginPage(string retrunUrl = null)
        {
            if (string.IsNullOrEmpty(retrunUrl))
                return RedirectToRoute("homepage", new { area = "" });
            return Redirect(retrunUrl);
        }

        [HttpGet]
        public ActionResult LogOut(string returnurl)
        {
            FormsAuthentication.SignOut();
            return RedirectFromLoginPage(returnurl);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return RedirectToAction("contact", "home");
            return View(new ForgotPasswordModel());
        }

        [HttpPost]
        public ActionResult SendForgotPassword(ForgotPasswordModel model)
        {
            Logger.Info("User attempting to Forgot Password: " + model.Email);
            bool accountExists = RavenSession.GetUserByCredentialId(model.Email) != null;
            if (accountExists)
            {
                IQueryable<AccountRecovery> accountRecoveryList =
                    RavenSession.Query<AccountRecovery>().Where(a => a.Email == model.Email);
                foreach (AccountRecovery accountRecovery in accountRecoveryList)
                {
                    RavenSession.Delete(accountRecovery);
                }
            }

            try
            {
                var recovery = new AccountRecovery(model.Email, model.GenerateToken());
                RavenSession.Store(recovery);
                _mailer.ForgotPasswordAttempt(recovery, accountExists,
                                              string.Format("{0}account/login/resetpassword?token={1}",
                                                            base.SiteConfig.CurrentEnvironmentConfigs["BaseUrl"], recovery.Token));
                model.ShowResult = "";
                model.Result = string.Format("We have sent an email to {0} with further instructions.", model.Email);
                return View("ForgotPassword", model);
            }
            catch (Exception ex)
            {
                Logger.Error("Error trying to send forgot password", ex);
                model.Result = string.Format("Error trying to send forgot password.");
                return View("ForgotPassword", model);
            }
        }

        [HttpGet]
        [RequireHttps]
        public ActionResult ResetPassword(string token)
        {
            Logger.Info("User attempting to Reset Password " + token);
            Guid recoveryToken;
            if (!Guid.TryParse(token.DecodeFrom64(), out recoveryToken))
            {
                ModelState.AddModelError("Token", "Invalid token, please try again");
                return View("ForgotPassword", new ForgotPasswordModel());
            }

            AccountRecovery accountRecovery = RavenSession.Query<AccountRecovery>().FirstOrDefault(a => a.Token == token);
            if (accountRecovery == null)
            {
                ModelState.AddModelError("Token", "Invalid token, please try again");
                return View("ForgotPassword", new ForgotPasswordModel());
            }

            if (DateTime.Now <= accountRecovery.ExpiresOn)
            {
                TempData["ChangePasswordForEmail"] = accountRecovery.Email;
                return View("ResetPassword", new ResetPasswordModel());
            }

            RavenSession.Delete(accountRecovery);
            ModelState.AddModelError("Token", "Invalid token, please try again");
            return View("ForgotPassword", new ForgotPasswordModel());
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = RavenSession.GetUserByCredentialId(TempData["ChangePasswordForEmail"].ToString());
            if (user == null)
            {
                ModelState.AddModelError("Error", "There was an error, please try again");
                return View("ForgotPassword", new ForgotPasswordModel());
            }

            try
            {
                user.UserCredentials.SetPassword(model.NewPassword);
                RavenSession.Store(user);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save new user password", ex);
                ModelState.AddModelError("Error", "There was an error saving your new password, please try again.");
                return View(model);
            }

            AccountRecovery accountRecovery =
                RavenSession.Query<AccountRecovery>().FirstOrDefault(
                    a => a.Email == TempData["ChangePasswordForEmail"].ToString());
            if (accountRecovery == null)
            {
                Logger.Error(string.Format("Failed to lookup account recovery token for user: {0}",
                                           TempData["ChangePasswordForEmail"]));
            }

            try
            {
                RavenSession.Delete(accountRecovery);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    string.Format("Failed to remove account recovery token ({0}) for user: {1}", accountRecovery.Token,
                                  accountRecovery.Email), ex);
            }

            _mailer.PasswordReset(TempData["ChangePasswordForEmail"].ToString());
            return RedirectToAction("Index", new { reset = true });
        }

        [ChildActionOnly]
        public ActionResult CurrentUser()
        {
            if (Request.IsAuthenticated == false)
                return View(new CurrentUserViewModel());

            User user = RavenSession.GetUserByCredentialId(HttpContext.User.Identity.Name);
            return
                View(new CurrentUserViewModel { FullName = user.FirstName + " " + user.LastName, IsAdmin = user.IsAdmin });
            // TODO: we don't really need a VM here
        }
    }
}