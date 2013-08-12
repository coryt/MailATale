using System.Web.Mvc;

namespace MAT.Web.Areas.Account
{
    public class AccountAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Account"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Account_default",
                "account/{controller}/{action}/{id}",
                new {controller = "Login", action = "Index", id = UrlParameter.Optional},
                new[] {"MAT.Web.Areas.Account.Controllers"}
                );
        }
    }
}