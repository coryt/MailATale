namespace MAT.Core.PaymentProcessor.BeanStream
{
    /// <summary>
    /// This is a fictitious interface just to imply a contract to a payment processors api. 
    /// </summary>
    public interface ISomePaymentApi
    {
        object BuildRequest();
        void AppendCustomerInfo(object request, object info);
        void AppendSubscriptionInfo(object request, object info);
        string ProcessRequest(object request);
    }
}