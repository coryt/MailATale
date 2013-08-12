namespace MAT.Core.InputModels
{
    public class SignUpLineItem
    {
        public SignUpLineItem()
        {
            Reader = new ReaderModel();
        }

        public ReaderModel Reader { get; set; }
        public string DeliverySchedule { get; set; }
        public string Subscription { get; set; }
    }
}