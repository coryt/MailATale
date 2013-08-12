using System.Web.Optimization;

namespace MAT.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                "~/assets/js/application.js",
                "~/assets/js/knockout_custom_bindings.js",
                "~/assets/js/knockout.validation.js",
                "~/assets/js/notifier.js",
                "~/assets/js/utility.js",
                "~/assets/js/mat_models.js",
                "~/assets/js/signup.js",
                "~/assets/js/gift.js",
                "~/assets/js/bootstrap-datepicker.js"
                ));

            bundles.Add(new StyleBundle("~/assets/css/site").Include(
                "~/assets/css/bootstrap.css",
                "~/assets/css/site.css"));
        }
    }
}