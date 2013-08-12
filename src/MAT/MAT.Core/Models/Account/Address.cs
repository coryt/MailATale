using System;
using MAT.Core.Models.Enumerations;

namespace MAT.Core.Models.Account
{
    public class Address : IEquatable<Address>
    {
        public Address()
        {
            Country = "Canada";
        }

        public Address(string addressType)
            : this()
        {
            AddressType = addressType;
        }

        public Address(AddressType addressType, string street1, string street2, string city, string province, string postalCode, string phone)
            : this(addressType.DisplayName)
        {
            Street1 = street1;
            Street2 = street2;
            City = city;
            Province = province;
            PostalCode = postalCode;
            Phone = phone;
        }

        public bool Equals(Address other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Street1, other.Street1, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(Street2, other.Street2, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(City, other.City, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(Province, other.Province, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(Country, other.Country, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(PostalCode, other.PostalCode, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Street1 != null ? Street1.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Street2 != null ? Street2.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Province != null ? Province.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Country != null ? Country.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PostalCode != null ? PostalCode.GetHashCode() : 0);
                return hashCode;
            }
        }

        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string AddressType { get; set; }
        public string FSA { get { return PostalCode.Substring(0, 3); } }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Address) obj);
        }

        public Address Copy()
        {
            return this.MemberwiseClone() as Address;
        }
    }
}