using System.Linq;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace MAT.Web.Infrastructure.Indexes
{
    public class Users_ByUserCredentialsEmail : AbstractIndexCreationTask<User>
    {
        public Users_ByUserCredentialsEmail()
        {
            Map = results => from result in results
                             select new { UserCredentials_Email = result.UserCredentials.Email };

            Index(x => x.UserCredentials.Email, FieldIndexing.Analyzed);
        }

    }
}