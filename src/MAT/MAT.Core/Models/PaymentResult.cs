
namespace MAT.Core.Models
{
    public class PaymentResponse
    {
        public PaymentResults Result { get; set; }
        public string Message { get; set; }

        public static PaymentResponse Denied()
        {
            return new PaymentResponse() { Result = PaymentResults.Denied, Message = "Your card was denied. Please review your payment information and address." };
        }

        public static PaymentResponse InvalidInput()
        {
            return new PaymentResponse() { Result = PaymentResults.InvalidInput, Message = string.Format("Check the following fields for errors.") };
        }

        public static PaymentResponse Success()
        {
            return new PaymentResponse() { Result = PaymentResults.Approved };
        }

        public static PaymentResponse Error()
        {
            return new PaymentResponse() { Result = PaymentResults.InternalError };
        }

        public static PaymentResponse CreateResponse(string transactionResponse)
        {
            switch (transactionResponse)
            {
                case "Success":
                    return Success();
                case "Denied":
                    return Denied();
                case "InvalidInput":
                    return InvalidInput();
                default:
                    return Error();
            }
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