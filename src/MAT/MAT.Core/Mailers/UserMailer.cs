using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using MAT.Core.InputModels;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Core.Models.Gift;
using MAT.Core.Models.Site;
using MAT.Core.Tasks;
using Mandrill.API;
using Mandrill.API.Endpoints;

namespace MAT.Core.Mailers
{
    public class UserMailer : IUserMailer
    {
        private SiteConfig _config;
        private string _apiKey;

        public UserMailer(SiteConfig siteConfig)
        {
            _config = siteConfig;
            _apiKey = _config.CurrentEnvironmentConfigs["Mailer.Mandrill.ApiKey"];
        }

        private void Send(
            string templateName,
            string to_email,
            object merge_vars,
            string text = "",
            string subject = "A Message From Mail A Tale",
            string from_email = "noreplay@mailatale.ca",
            string from_name = "MailATale.ca",
            string to_name = null,
            object headers = null,
            bool track_opens = true,
            bool track_clicks = true,
            bool auto_text = true,
            bool url_strip_qs = true,
            string bcc_address = "",
            bool merge = false,
            object[] global_merge_vars = null,
            object[] tags = null,
            object[] google_analytics_domains = null,
            object[] google_analytics_campaign = null,
            object[] metadata = null,
            object[] attachments = null)
        {
            var to = to_name != null ? (object)new { email = to_email, name = to_name } : new { email = to_email };
            headers = headers ?? new { };
            global_merge_vars = global_merge_vars ?? new[] { new { } };
            tags = tags ?? new object[] { };
            google_analytics_domains = google_analytics_domains ?? new object[] { };
            google_analytics_campaign = google_analytics_campaign ?? new object[] { };
            metadata = metadata ?? new object[] { };
            attachments = attachments ?? new object[] { };

            MandrillRequest.With(_apiKey).In<Messages>().sendtemplate(templateName, new List<object>(), new
                {
                    text = text,
                    subject = subject,
                    from_email = from_email,
                    from_name = from_name,
                    to = new object[] { to },
                    headers = headers,
                    track_opens = track_opens,
                    track_clicks = track_clicks,
                    auto_text = auto_text,
                    url_strip_qs = url_strip_qs,
                    bcc_address = bcc_address,
                    merge = merge,
                    global_merge_vars = global_merge_vars,
                    merge_vars = new[] { merge_vars },
                    tags = tags,
                    google_analytics_domains = google_analytics_domains,
                    google_analytics_campaign = google_analytics_campaign,
                    metadata = metadata,
                    attachments = attachments
                });
        }

        public void General(string email, string subject = "A Message From Mail A Tale", string message = "")
        {
            Send(templateName: "GenericEmailTemplate",
                 to_email: email,
                 merge_vars: new { rcpt = "hello@mailatale.ca", vars = new[] { new { name = "SUBJECT", content = subject }, new { name = "MessageContent", content = message } } });
        }

        private string GenerateReceipt(Order order)
        {
            var paymentTable = string.Format(
                @"<table style=""width:100%"" cellpadding=""1"" cellspacing=""1""><tbody><tr><td><font size=""2"" face=""Arial, Arial, sans-serif"">
                 </font></td></tr><tr>
                     <td style=""padding-left:10px"" colspan=""2"" bgcolor=""#474241""><font size=""4"" color=""#ffffff"">Transaction Details:</font></td>
                 </tr>
                 <tr>
                     <td style=""padding-left:10px;padding-top:5px"" width=""50%"" valign=""top""><b>Order Date:</b></td>
                     <td style=""padding-left:10px;padding-top:5px"" width=""50%"" valign=""top"">{0}</td>
                 </tr>
                 <tr>
                     <td style=""padding-left:10px;padding-top:5px"" width=""50%"" valign=""top""><b>Order Number:</b></td>
                     <td style=""padding-left:10px;padding-top:5px"" width=""50%"" valign=""top"">{1}</td>
                 </tr>
                 <tr>
                     <td style=""padding-left:10px;padding-top:5px"" width=""50%"" valign=""top""><b>Bank Auth Number:</b></td>
                     <td style=""padding-left:10px;padding-top:5px"" width=""50%"" valign=""top"">{2}</td>
                 </tr>
                 <tr>
                     <td style=""padding-left:10px;padding-top:5px"" width=""50%"" valign=""top""><b>Order Total:</b></td>
                     <td style=""padding-left:10px;padding-top:5px"" width=""50%"" valign=""top"">{3} CAD</td>
                 </tr>
                 <tr>
                     <td style=""padding-left:10px;padding-top:5px"" width=""50%"" valign=""top""><b>Name on Card:</b></td>
                     <td style=""padding-left:10px;padding-top:5px"" width=""50%"" valign=""top"">{4}</td>
                 </tr>
                
             </tbody></table>", order.DateCreated.ToString(), order.Id, order.TransactionResponse.AuthCode, order.Total, order.CreditCardName);

            var billingAddressTable =
                string.Format(@"<table style=""width:100%;border-collapse:collapse""><tbody><tr><td><font size=""2"" face=""Arial, Arial, sans-serif"">
                        </font></td></tr>
                        <tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">{0}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">{1}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">{2}&nbsp;{3}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">{4}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">CA</td>
                        </tr>
                        <tr>
                            <td></td>
                           
                        </tr>
                        
           		    </tbody></table>", order.BillingAddress.Street1, order.BillingAddress.Street2,
                              order.BillingAddress.City, order.BillingAddress.Province, order.BillingAddress.PostalCode);

            var shippingAddressTable =
                string.Format(
                    @"<table style=""empty-cells:show;width:100%;min-height:100%;border-collapse:collapse""><tbody><tr><td><font size=""2"" face=""Arial, Arial, sans-serif"">
                        </font></td></tr><tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">{0}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">{1}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">{2}&nbsp;{3}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">{4}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">CA</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style=""padding-left:10px;padding-top:2px"" valign=""top"">&nbsp;</td>
                        </tr>
           		 </tbody></table>", order.ShippingAddress.Street1, order.ShippingAddress.Street2,
                              order.ShippingAddress.City, order.ShippingAddress.Province, order.ShippingAddress.PostalCode);

            var table = @"<table style=""width:600px;margin:0 auto 0"" cellspacing=""1""><tbody><tr><td><font size=""2"" face=""Arial, Arial, sans-serif"">
    </font></td></tr><tr>
        <td style=""border-bottom-color:black;border-bottom-width:2px;border-bottom-style:solid"">
        
            
        	<table style=""width:100%;border-bottom-color:black;border-bottom-width:2px;border-bottom-style:solid"" cellpadding=""1"" cellspacing=""1""><tbody><tr><td><font size=""2"" face=""Arial, Arial, sans-serif"">
				</font></td></tr><tr>
					<td style=""padding-top:10px;padding-left:10px;border-bottom-color:black;border-bottom-width:2px;border-bottom-style:solid"" colspan=""2""><font size=""5"">MAIL A TALE PURCHASE RECEIPT</font></td>
                </tr>
                <tr>
                	<td> </td>
                </tr>
                <tr>
                    <td style=""padding-left:10px"" colspan=""2"" bgcolor=""#474241""><font size=""4"" color=""#ffffff"">Company Information:</font></td>
                </tr>
                <tr>
                    <td></td>
                    <td style=""padding-left:10px;padding-top:2px"" width=""50%"" valign=""top""><a href=""http://www.mailatale.ca"" target=""_blank"">http://www.mailatale.ca</a></td>
                 </tr>
                 <tr>
                     <td></td>
                     <td style=""padding-left:10px;padding-top:2px"" width=""50%"" valign=""top"">Mailatale</td>
                 </tr>
                
                 	<tr>
					<td colspan=""2""></td>
                </tr>
           </tbody>
        </table>
           {0}
             <table style=""width:100%"">
             	<tbody><tr>
                	<td></td>
                </tr>
             </tbody></table>       
            
                   
             <table style=""width:100%;border-top-color:black;border-top-width:2px;border-top-style:solid"">  
                <tbody><tr>
                  <td style=""padding-left:10px"" colspan=""2"" bgcolor=""#474241""><font size=""4"" color=""#ffffff"" face=""Arial, Arial, sans-serif"">Bill to:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<wbr>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Ship To:</font></td>
                 </tr>
             	 <tr>
                	<td valign=""top"">
                    
					{1}
                    </td>
                        
                    <td>
                    
            	    {2}
                 
				 </td>
              </tr>
          </tbody></table>
      
          </td>
      </tr>
</tbody></table>";

            return string.Format(table, paymentTable, billingAddressTable, shippingAddressTable);
        }
        public void Welcome(User user, Order order)
        {
            var receipt = GenerateReceipt(order);

            MandrillRequest.With(_apiKey).In<Messages>().
            sendtemplate("WelcomeEmailTemplate", new List<object>(), new
            {
                text = "",
                subject = "Welcome To Mail A Tale!",
                from_email = "noreply@mailatale.ca",
                from_name = "The Mail A Tale Team",
                to = new object[] { new { email = user.Email, name = user.FirstName } },
                headers = new { },
                track_opens = true,
                track_clicks = true,
                auto_text = true,
                url_strip_qs = true,
                bcc_address = "",
                merge = false,
                global_merge_vars = new[] { new { } },
                merge_vars = new[] { new { rcpt = user.Email, vars = new[] { new { name = "SUBJECT", content = "Welcome To Mail A Tale!" }, new { name = "CustomerName", content = user.FirstName }, new { name = "RECEIPT", content = receipt } } } },
                tags = new object[] { },
                google_analytics_domains = new object[] { },
                google_analytics_campaign = new object[] { },
                metadata = new object[] { },
                attachments = new object[] { }
            });
        }

        public void WelcomeGift(GiftPurchase purchase, string redeemLink, string personalMessage)
        {
            var subscriptionType = string.Format("{0} ({1})", purchase.SubscriptionProduct.Name, purchase.SubscriptionProduct.Price.ToString("C"));
            var duration = string.Format("{0} month{1}", purchase.SubscriptionLength, purchase.SubscriptionLength == 1 ? "" : "s");

            const string subject = "You've received a gift from MailATale.ca!";
            string subscription = string.Format("<p><strong>Gift giver</strong>: {0}</p><p><strong>Subscription Type</strong>: {1}</p><p><strong>Duration</strong>: {2}</p>",
                                                purchase.SenderName, subscriptionType, duration);
            
            Send(templateName: "WelcomeGiftEmailTemplate",
                 to_email: purchase.RecipientEmail,
                 merge_vars: new
                 {
                     rcpt = purchase.RecipientEmail,
                     vars = new[] { 
                         new { name = "SUBJECT", content = subject }, 
                         new { name = "Subscription", content = subscription }, 
                         new { name = "RedeemLink", content = redeemLink }, 
                         new { name = "CustomerName", content = purchase.RecipientName },
                         new { name = "PERSONALMESSAGE", content = personalMessage }
                     }
                 },
                 subject: subject);
        }

        public void PurchaseGift(string senderEmail)
        {
            Send(templateName: "PurchaseGiftEmailTemplate",
                 to_email: senderEmail,
                 merge_vars: new
                         {
                             rcpt = senderEmail,
                             vars = new[] { new { name = "SUBJECT", content = "Thank You from MailATale.ca" } }
                         },
                 subject: "Thank You from MailATale.ca");
        }

        public void PasswordReset(string email)
        {
            MandrillRequest.With(_apiKey).In<Messages>().
                sendtemplate("PasswordResetTemplate", new List<object>(), new
                {
                    text = "",
                    subject = "MailATale.ca Password Reset",
                    from_email = "noreply@mailatale.ca",
                    from_name = "MailATale.ca",
                    to = new object[] { new { email = email, name = email } },
                    headers = new { },
                    track_opens = true,
                    track_clicks = true,
                    auto_text = true,
                    url_strip_qs = true,
                    bcc_address = "",
                    merge = false,
                    global_merge_vars = new[] { new { } },
                    merge_vars = new[] { new { rcpt = email, vars = new[] { new { name = "SUBJECT", content = "Password Reset" } } } },
                    tags = new object[] { },
                    google_analytics_domains = new object[] { },
                    google_analytics_campaign = new object[] { },
                    metadata = new object[] { },
                    attachments = new object[] { }
                });
        }

        public void ForgotPasswordAttempt(AccountRecovery recovery, bool accountExists, string resetLink)
        {
            MandrillRequest.With(_apiKey).In<Messages>().
                sendtemplate("ForgotPasswordTemplate", new List<object>(), new
                {
                    text = "",
                    subject = "MailATale.ca Forgot Password",
                    from_email = "noreply@mailatale.ca",
                    from_name = "MailATale.ca",
                    to = new object[] { new { email = recovery.Email, name = recovery.Email } },
                    headers = new { },
                    track_opens = true,
                    track_clicks = true,
                    auto_text = true,
                    url_strip_qs = true,
                    bcc_address = "",
                    merge = false,
                    global_merge_vars = new[] { new { } },
                    merge_vars = new[] { new { rcpt = recovery.Email, vars = new[] { new { name = "SUBJECT", content = "Password Reset" }, new { name = "ResetPasswordURL", content = resetLink } } } },
                    tags = new object[] { },
                    google_analytics_domains = new object[] { },
                    google_analytics_campaign = new object[] { },
                    metadata = new object[] { },
                    attachments = new object[] { }
                });
        }

        public void InternalContactForm(ContactModel model)
        {
            var htmlContent = string.Format("<p><strong>Name</strong>:{0}</p><p><strong>Email</strong>:{1}</p><p><strong>Message</strong>:{2}</p>", model.Name, model.Email, model.Message);

            MandrillRequest.With(_apiKey).In<Messages>().
                sendtemplate("GenericEmailTemplate", new List<object>(), new
                    {
                        text = "",
                        subject = "Website Contact Request",
                        from_email = model.Email,
                        from_name = model.Name,
                        to = new object[] { new { email = "hello@mailatale.ca", name = "MailATale.ca" } },
                        headers = new { },
                        track_opens = true,
                        track_clicks = true,
                        auto_text = true,
                        url_strip_qs = true,
                        bcc_address = "",
                        merge = false,
                        global_merge_vars = new[] { new { } },
                        merge_vars = new[] { new { rcpt = "hello@mailatale.ca", vars = new[] { new { name = "MessageContent", content = htmlContent }, new { name = "SUBJECT", content = "Website Contact" } } } },
                        tags = new object[] { },
                        google_analytics_domains = new object[] { },
                        google_analytics_campaign = new object[] { },
                        metadata = new object[] { },
                        attachments = new object[] { }
                    });
        }

        public void InternalGeneral(string message)
        {
            MandrillRequest.With(_apiKey).In<Messages>().
               sendtemplate("GenericEmailTemplate", new List<object>(), new
               {
                   text = "",
                   subject = "A Message From Mail A Tale",
                   from_email = "noreply@mailatale.ca",
                   from_name = "MailATale.ca",
                   to = new object[] { new { email = "hello@mailatale.ca" } },
                   headers = new { },
                   track_opens = true,
                   track_clicks = true,
                   auto_text = true,
                   url_strip_qs = true,
                   bcc_address = "",
                   merge = false,
                   global_merge_vars = new[] { new { } },
                   merge_vars = new[] { new { rcpt = "hello@mailatale.ca", vars = new[] { new { name = "SUBJECT", content = "A Message From Mail A Tale" }, new { name = "MessageContent", content = message } } } },
                   tags = new object[] { },
                   google_analytics_domains = new object[] { },
                   google_analytics_campaign = new object[] { },
                   metadata = new object[] { },
                   attachments = new object[] { }
               });
        }
    }
}