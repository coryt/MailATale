using System;
using System.Linq;
using MAT.Core.InputModels;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.Subscription;

namespace MAT.Core.Models.Gift
{
    public class GiftPurchase
    {
        public GiftPurchase()
        {
        }

        public GiftPurchase(string email, string name, GiftPurchaseLineItem lineItem, SubscriptionProduct selectedPackage, string personalMessage, DateTime sendGiftNoticeOn)
        {
            RedeemId = Guid.NewGuid();

            SenderEmail = email;
            SenderName = name;

            RecipientName = lineItem.Recipient.Name;
            RecipientEmail = lineItem.Recipient.Email;
            RecipientProvince = Enumeration.FromValue<Province>(Convert.ToInt32(lineItem.Recipient.Province)).DisplayName;
            RecipientReaderName = lineItem.Recipient.ReaderName;
            RecipientReaderGender = lineItem.Recipient.Gender;

            SubscriptionLength = lineItem.GiftLength;
            SubscriptionProduct = selectedPackage;

            PersonalMessage = personalMessage;
            SendGiftNoticeOn = sendGiftNoticeOn;
        }

        public string Id { get; set; }
        public bool Purchased { get; set; }
        public DateTime DatePurchased { get; set; }

        public Guid RedeemId { get; set; }

        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string PersonalMessage { get; set; }
        public DateTime SendGiftNoticeOn { get; set; }

        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientProvince { get; set; }
        public string RecipientReaderName { get; set; }
        public string RecipientReaderGender { get; set; }

        public int SubscriptionLength { get; set; }
        public SubscriptionProduct SubscriptionProduct { get; set; }

        public bool Redeemed { get; set; }
        public DateTime DateRedeemed { get; set; }
        public string RedeemedByUserId { get; set; }
        protected string UserScriptionId { get; set; }
        public bool GiftNoticeSent { get; set; }

        public void RedeemedBy(User user, UserSubscription subscription)
        {
            Redeemed = true;
            DateRedeemed = DateTime.Now;
            RedeemedByUserId = user.Id;
            UserScriptionId = subscription.Id;
        }
    }
}