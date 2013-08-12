using MAT.Core.Models;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;

namespace MAT.Core.InputModels
{
    public class AddressModel
    {
        public AddressModel()
        {
        }

        public AddressModel(string addressType)
        {
            AddressType = addressType;
        }

        public AddressModel(Address address)
        {
            Street1 = address.Street1;
            Street2 = address.Street2;
            City = address.City;
            Province = Models.Enumerations.Province.FindByAbbreviation(address.Province).Value;
            PostalCode = address.PostalCode;
            Phone = address.Phone;
            AddressType = address.AddressType;
        }

        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public int Province { get; set; }
        public string Country { get { return "Canada"; } }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string AddressType { get; set; }

        public Address ToAddress()
        {
            var addressType = Enumeration.FromDisplayName<AddressType>(AddressType);
            var province = Enumeration.FromValue<Province>(Province).DisplayName;
            return new Address(addressType, Street1, Street2, City, province, PostalCode, Phone);
        }
    }
}