using System.Linq;
using MAT.Core.Models;
using MAT.Core.Models.Subscription;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace MAT.Web.Infrastructure.Indexes
{
    public class UserSubscription_ByUserId : AbstractIndexCreationTask<UserSubscription>
    {
        public UserSubscription_ByUserId()
        {
            Map = results => from result in results
                             select new { result.UserId };

            Index(x => x.UserId, FieldIndexing.Analyzed);
        }
    }
}