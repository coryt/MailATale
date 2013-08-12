using MAT.Proxy.PaymentProcessor.BeanStream;

namespace MAT.Core.Models
{
    public class PaymentResponse
    {
        public PaymentResults Result { get; set; }
        public TransactionResponse Response { get; set; }
        public string Message { get; set; }

        public static PaymentResponse ExternalError(TransactionResponse response)
        {
            return new PaymentResponse() { Result = PaymentResults.ExternalError, Message = "There was a system error. Please try again later.", Response = response };
        }

        public static PaymentResponse Denied(TransactionResponse response)
        {
            return new PaymentResponse() { Result = PaymentResults.Denied, Message = "Your card was denied. Please review your payment information and address.", Response = response };
        }

        public static PaymentResponse Success(TransactionResponse response)
        {
            return new PaymentResponse() { Result = PaymentResults.Approved, Response = response };
        }

        public static PaymentResponse Error(TransactionResponse response)
        {
            return new PaymentResponse() { Result = PaymentResults.InternalError, Message = response.MessageText, Response = response };
        }

        public static PaymentResponse InvalidInput(string message)
        {
            return new PaymentResponse() { Result = PaymentResults.InvalidInput, Message = message, Response = null };
        }

        public static PaymentResponse InvalidInput(TransactionResponse transactionResponse)
        {
            return new PaymentResponse() { Result = PaymentResults.InvalidInput, Message = string.Format("Check the following fields for errors: {0}", transactionResponse.ErrorFields), Response = null };
        }

        public static PaymentResponse Success()
        {
            return new PaymentResponse() { Result = PaymentResults.Approved, Response = null };
        }

        public static PaymentResponse Error(string response)
        {
            return new PaymentResponse() { Result = PaymentResults.InternalError, Message = response, Response = null };
        }
    }

    public enum PaymentResults
    {
        Approved = 0,
        Denied = 1,
        InternalError = 2,
        ExternalError = 3,
        InvalidInput = 4
    }
}