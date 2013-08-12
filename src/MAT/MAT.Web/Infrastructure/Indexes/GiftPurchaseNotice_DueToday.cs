using System;
using System.Linq;
using MAT.Core.Models.Gift;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace MAT.Web.Infrastructure.Indexes
{
    public class GiftPurchaseNotice_DueToday : AbstractIndexCreationTask<GiftPurchase>
    {
        public GiftPurchaseNotice_DueToday()
        {
            Map = docs => from gift in docs
                          where gift.SendGiftNoticeOn.Date <= DateTime.Today.Date && !gift.GiftNoticeSent && gift.Purchased
                          select new { UserId = gift.Id };

            Index(x => x.Id, FieldIndexing.Analyzed);
        }
    }
}