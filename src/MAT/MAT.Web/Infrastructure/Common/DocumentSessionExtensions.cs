using System.Collections.Generic;
using System.Linq;
using System.Web;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Web.Infrastructure.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace MAT.Web.Infrastructure.Common
{
    public static class DocumentSessionExtensions
    {
        public static User GetCurrentUser(this IDocumentSession session)
        {
            if (HttpContext.Current.Request.IsAuthenticated == false)
                return null;

            string email = HttpContext.Current.User.Identity.Name;
            var user = session.GetUserByCredentialId(email);
            return user;
        }

        public static User GetUserByCredentialId(this IDocumentSession session, string email)
        {
            return session.Query<User, Users_ByUserCredentialsEmail>().FirstOrDefault(u => u.UserCredentials.Email == email);
        }

        public static List<PreferenceQuestion> GetSignupPreferenceQuestions(this IDocumentSession session)
        {
            return session.Query<PreferenceQuestion>().Where(p => p.IncludeInSignup).OrderBy(p=>p.Category).ToList();
        }
    }
}