using System;
using System.ComponentModel.DataAnnotations;

namespace MAT.Core.InputModels
{
    public class GiftRecipientModel
    {
        public GiftRecipientModel() : this(null)
        {
        }

        public GiftRecipientModel(string giftPurchaseId)
        {
            AccountInfo = new RegisterAccountModel();
            ReaderInfo = new ReaderModel();
            GiftPurchaseId = giftPurchaseId;
        }

        [Required]
        public string GiftPurchaseId { get; set; }

        [Required]
        public RegisterAccountModel AccountInfo { get; set; }

        [Required]
        public ReaderModel ReaderInfo { get; set; }
    }
}
