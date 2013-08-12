using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Configuration;

namespace MAT.Core.Models.Site
{
    public class SiteConfig : Model
    {
        [Required]
        [Display(Name = "Site title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Admin Email")]
        public string AdminEmail { get; set; }

        [Display(Name = "Slogan")]
        public string Subtitle { get; set; }

        [Display(Name = "Google-Analytics Key")]
        public string GoogleAnalyticsKey { get; set; }

        [Display(Name = "MetaDescription")]
        public string MetaDescription { get; set; }

        [Display(Name = "Environment")]
        public string Environment { get; set; }

        [Display(Name = "EnvironmentConfigs")]
        public Dictionary<string, string> CurrentEnvironmentConfigs { get { return EnvironmentConfigs[Environment]; } }

        public SiteConfig()
        {
        }

        public SiteConfig(string environment)
        {
            Environment = environment ?? WebConfigurationManager.AppSettings["Environment"];
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AdminEmail != null ? AdminEmail.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Subtitle != null ? Subtitle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (GoogleAnalyticsKey != null ? GoogleAnalyticsKey.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MetaDescription != null ? MetaDescription.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Environment != null ? Environment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CurrentEnvironmentConfigs != null ? CurrentEnvironmentConfigs.GetHashCode() : 0);
                return hashCode;
            }
        }

        public bool Equals(SiteConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Title, other.Title, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(AdminEmail, other.AdminEmail, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(Subtitle, other.Subtitle, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(GoogleAnalyticsKey, other.GoogleAnalyticsKey, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(MetaDescription, other.MetaDescription, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(Environment, other.Environment, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.BaseUrl"], other.CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.BaseUrl"], StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.Passcode"], other.CurrentEnvironmentConfigs["PaymentProcessing.BeanStream.Passcode"], StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(CurrentEnvironmentConfigs["Mailer.Mandrill.ApiKey"], other.CurrentEnvironmentConfigs["Mailer.Mandrill.ApiKey"], StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SiteConfig)obj);
        }

        public static SiteConfig InitalizeSiteConfig(string currentEnvironment)
        {
            var devConfigs = new Dictionary<string, string>()
                {
                    {"ReCaptchaPrivateKey", "6Le1ZcMSAAAAAC9VnxL6908-iMk7SNGLq3CmLF8V"},
                    {"ReCaptchaPublicKey", "6Le1ZcMSAAAAANGfy8yHQk02A6U-3GenLHjz7VHQ"},
                    {"BaseUrl", "http://172.20.10.14/"},
                    {"SecureBaseUrl", "https://172.20.10.14/"},
                    {"PaymentProcessing.BeanStream.MerchantId", "252390000"},
                    {"PaymentProcessing.BeanStream.Username", "mailatale12"},
                    {"PaymentProcessing.BeanStream.Password", "Ce774aD2"},
                    {"PaymentProcessing.BeanStream.Passcode", "6E0c304e3BAc4df680eFb7E4E07F740E"},
                    {"Mailer.Mandrill.ApiKey", "c7a9ac77-dae0-4a28-b1d7-34bab8c1c85c"},
                    {"MaintenanceMode", "true"}
                };

            var stageConfigs = new Dictionary<string, string>()
                {
                    {"ReCaptchaPrivateKey", "6Le1ZcMSAAAAAC9VnxL6908-iMk7SNGLq3CmLF8V"},
                    {"ReCaptchaPublicKey", "6Le1ZcMSAAAAANGfy8yHQk02A6U-3GenLHjz7VHQ"},
                    {"BaseUrl", "http://staging.mailatale.ca/"},
                    {"SecureBaseUrl", "https://staging.mailatale.ca/"},
                    {"PaymentProcessing.BeanStream.MerchantId", "252390000"},
                    {"PaymentProcessing.BeanStream.Username", "mailatale12"},
                    {"PaymentProcessing.BeanStream.Password", "Ce774aD2"},
                    {"PaymentProcessing.BeanStream.Passcode", "6E0c304e3BAc4df680eFb7E4E07F740E"},
                    {"Mailer.Mandrill.ApiKey", "c7a9ac77-dae0-4a28-b1d7-34bab8c1c85c"},
                    {"MaintenanceMode", "true"}
                };

            var prodConfigs = new Dictionary<string, string>()
                {
                    {"ReCaptchaPrivateKey", "6Le1ZcMSAAAAAC9VnxL6908-iMk7SNGLq3CmLF8V"},
                    {"ReCaptchaPublicKey", "6Le1ZcMSAAAAANGfy8yHQk02A6U-3GenLHjz7VHQ"},
                    {"BaseUrl", "http://www.mailatale.ca/"},
                    {"SecureBaseUrl", "https://www.mailatale.ca/"},
                    {"PaymentProcessing.BeanStream.MerchantId", "233530000"},
                    {"PaymentProcessing.BeanStream.Username", "mailatale12"},
                    {"PaymentProcessing.BeanStream.Password", "8z57jRBwdYIx"},
                    {"PaymentProcessing.BeanStream.Passcode", "65b3Af2B020549c7919e53Cd8D0012d0"},
                    {"Mailer.Mandrill.ApiKey", "c7a9ac77-dae0-4a28-b1d7-34bab8c1c85c"},
                    {"MaintenanceMode", "true"}
                };

            return new SiteConfig(currentEnvironment)
            {
                Id = "Site/Config",
                EnvironmentConfigs = new Dictionary<string, Dictionary<string, string>> { { "dev", devConfigs }, { "stage", stageConfigs }, { "prod", prodConfigs } },
            };
        }

        public Dictionary<string, Dictionary<string, string>> EnvironmentConfigs { get; set; }

        public override string ToString()
        {
            return string.Format("SiteConfig for {0}. Maintenance Mode:{1}", Environment, Convert.ToBoolean(CurrentEnvironmentConfigs["MaintenanceMode"]) ? "Enabled" : "Disabled");
        }
    }
}