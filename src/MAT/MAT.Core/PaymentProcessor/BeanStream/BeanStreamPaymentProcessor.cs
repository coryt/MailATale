using MAT.Core.Models;
using NLog;

namespace MAT.Core.PaymentProcessor.BeanStream
{
    public class BeanStreamPaymentProcessor : IPaymentProcessor
    {
        private readonly Logger _log = LogManager.GetLogger(typeof(BeanStreamPaymentProcessor).Name);
        private readonly ISomePaymentApi _paymentApi;

        public BeanStreamPaymentProcessor(ISomePaymentApi paymentApi)
        {
            _paymentApi = paymentApi;
        }

        public PaymentResponse ProcessPayment(Order subscriptionOrder)
        {
            _log.Info(string.Format("Attempting to Process Transaction"));
            var request = _paymentApi.BuildRequest();
            _paymentApi.AppendCustomerInfo(request, subscriptionOrder.BillingAddress);
            _paymentApi.AppendSubscriptionInfo(request, subscriptionOrder);

            var transactionResponse = _paymentApi.ProcessRequest(request);

            return PaymentResponse.CreateResponse(transactionResponse);
        }
    }
}