using System.Collections.Generic;
using System.Web.Mvc;
using MAT.Core.Models;
using MAT.Core.Models.Subscription;

namespace MAT.Core.InputModels
{
    public class AccountSettingsModel
    {
        public ChangePasswordModel ChangePasswordModel { get; set; }

        public List<Reader> Readers { get; set; }

        public PaymentInfoModel PaymentModel { get; set; }

        public AccountSettingsModel()
        {
            ChangePasswordModel = new ChangePasswordModel();
            PaymentModel = new PaymentInfoModel();
            Readers = new List<Reader>();
        }
    }
}