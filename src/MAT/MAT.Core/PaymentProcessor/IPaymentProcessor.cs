using MAT.Core.InputModels;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Core.Models.Gift;
using MAT.Core.Models.SignUp;

namespace MAT.Core.PaymentProcessor
{
    public interface IPaymentProcessor
    {
        PaymentResponse ProcessPayment(SubscriptionOrder subscriptionOrder);
        PaymentResponse ProcessPayment(GiftOrder order);

        PaymentResponse ModifyAccount(string userPaymentProviderId, User userInfo, Address billingAddress, Address shippingAddress,
                                      PaymentInfoModel paymentInfoModel);

        PaymentResponse ModifyAccount(User userInfo, Address billingAddress, Address shippingAddress,
                                      PaymentInfoModel paymentInfoModel);
    }
}