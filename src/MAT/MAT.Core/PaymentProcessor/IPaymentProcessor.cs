using MAT.Core.Models;

namespace MAT.Core.PaymentProcessor
{
    public interface IPaymentProcessor
    {
        PaymentResponse ProcessPayment(Order subscriptionOrder);
    }
}