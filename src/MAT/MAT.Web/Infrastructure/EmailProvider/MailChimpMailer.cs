using System.Configuration;

namespace MAT.Web.Infrastructure.EmailProvider
{
    public class MailChimpMailer
    {
        private readonly string _apiKey;
        private readonly string _listId;

        public MailChimpMailer()
        {
            _apiKey = ConfigurationManager.AppSettings["MailChimp.APIKey"];
            _listId = ConfigurationManager.AppSettings["MailChimp.AllUsersListId"];
        }

        public MailChimpMailer(string apiKey, string listId)
        {
            _apiKey = apiKey;
            _listId = listId;
        }

        public void SubscribeUserToList(string email)
        {
             listSubscribe cmd = new listSubscribe();
             listSubscribeParms newlistSubscribeParms = new listSubscribeParms
             {
                 apikey = _apiKey,
                 id = _listId,
                 email_address = email,
                 double_optin = false,
                 email_type = EnumValues.emailType.html,
                 replace_interests = true,
                 send_welcome = false,
                 update_existing = true,

             };
             listSubscribeInput newlistSubscribeInput = new listSubscribeInput(newlistSubscribeParms);
             var result = cmd.Execute(newlistSubscribeInput);
         }
    }
}