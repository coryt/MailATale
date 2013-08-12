using System.Linq;
using Castle.Core.Logging;
using MAT.Core.Mailers;
using MAT.Core.Models.Gift;
using MAT.Core.Models.Site;
using MAT.Core.Tasks;

namespace MAT.Web.Infrastructure.Tasks
{
    public class GiftEmailerTask : BackgroundTask
    {
        private readonly SiteConfig _config;
        private readonly IUserMailer _mailer;
        private readonly ILogger _logger;


        public GiftEmailerTask(SiteConfig config, IUserMailer mailer, ILogger logger)
        {
            _config = config;
            _mailer = mailer;
            _logger = logger;
        }

        public override void Execute()
        {
            _logger.InfoFormat("Starting GiftEmailer Execution");

            var gifts = (DocumentSession.Query<GiftPurchase>("GiftPurchaseNotice/DueToday")).ToList();

            _logger.InfoFormat("{0} gifts found", gifts.Count);

            foreach (var giftPurchase in gifts)
            {
                var id = giftPurchase.RedeemId.ToString("N").ToLowerInvariant();
                var link = string.Format("{0}gift/redeem?id={1}", _config.CurrentEnvironmentConfigs["SecureBaseUrl"], id);
                _mailer.WelcomeGift(giftPurchase, link, giftPurchase.PersonalMessage);

                giftPurchase.GiftNoticeSent = true;
                DocumentSession.Store(giftPurchase);
                DocumentSession.SaveChanges();
            }
            
        }
    }
}