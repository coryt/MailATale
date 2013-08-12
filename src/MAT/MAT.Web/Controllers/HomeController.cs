using System;
using System.Web;
using System.Web.Mvc;
using MAT.Core.InputModels;
using MAT.Core.Mailers;
using MAT.Core.Models.Account;
using MAT.Web.Infrastructure.Common;
using MAT.Web.ViewModels;

namespace MAT.Web.Controllers
{
    public class HomeController : MATController
    {
        private readonly IUserMailer _userMailer;

        public HomeController(IUserMailer userMailer)
        {
            _userMailer = userMailer;
        }

        public ActionResult Index()
        {
            Logger.Info("Hello");
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Result = TempData["Result"];
            return View();
        }

        [HttpPost]
        public ActionResult ContactSubmit(ContactModel model)
        {
            try
            {
                Logger.Info(string.Format("Message from {0} ({1}). Subject:{2} Message:{3}", model.Name, model.Email,
                                          model.Subject, model.Message));
                _userMailer.InternalContactForm(model);
                TempData["Result"] =
                    "<div class=\"alert alert-success\">Thank you for contacting Mailatale, we will get back to you as soon as possible!</div>";
            }
            catch (Exception exception)
            {
                Logger.Fatal("Failed to send contact form. " + exception.Message);
                TempData["Result"] =
                    "<div class=\"alert\">We're sorry, we could not send your message. The problem has been logged. Please don't let this stop you from contacting us at hello@mailatale.ca. We'd love to hear from you!</div>";
            }
            return RedirectToAction("Contact");
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult HowItWorks()
        {
            return View();
        }

       

        [ChildActionOnly]
        public ActionResult AccountPanel()
        {
            User user = RavenSession.GetCurrentUser();

            var vm = new CurrentUserViewModel();
            if (user != null)
            {
                vm.FullName = user.FirstName + " " + user.LastName;
                vm.IsAdmin = user.IsAdmin;
            }
            return View("_LoginPartial", vm);
        }
    }
}