using System.Collections.Generic;
using MAT.Core.Models;

namespace MAT.Core.InputModels
{
    public class SignUpModel
    {
        public List<SignUpLineItem> Lines { get; set; }
        public RegisterAccountModel Account { get; set; }
        public PaymentInfoModel Payment { get; set; }
        public ShippingAreas Area { get; set; }
        public decimal Discount { get; set; }
        public string ReferralName { get; set; }
        public int Readers { get; set; }

        public SignUpModel()
        {
            Readers = 0;
            Lines = new List<SignUpLineItem>();
            Payment = new PaymentInfoModel();
            Account = new RegisterAccountModel();
        }
    }
}