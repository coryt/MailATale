using System;
using MAT.Core.InputModels;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Core.Models.Gift;

namespace MAT.Core.Mailers
{ 
    public interface IUserMailer
    {
        void Welcome(User user, Order order);
        void WelcomeGift(GiftPurchase purchase, string redeemLink, string personalMessage);
        void PurchaseGift(string senderEmail);
        void ForgotPasswordAttempt(AccountRecovery recovery, bool accountExists, string resetLink);
        void PasswordReset(string email);
        void General(string email, string subject = "A Message From Mail A Tale", string message = "");
        
        void InternalContactForm(ContactModel model);
        void InternalGeneral(string message);
    }
}