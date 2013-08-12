using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MAT.Core.Models.Enumerations;

namespace MAT.Core.Models.Account
{
    public class User
    {
        public User()
        {
            Addresses = new List<Address>();
            PaymentProviderIds = new List<string>();
            Subscriptions = new List<string>();
            Readers = new List<dynamic>();
        }

        public User(string firstName, string lastName, string email, bool enabled)
            : this()
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Enabled = enabled;
            CreatedOn = DateTime.Now;
        }

        public User(string firstName, string lastName, string email, bool enabled, DateTime dateAdded)
            : this(firstName, lastName, email, enabled)
        {
            CreatedOn = dateAdded;
        }

        [DisplayName("ID")]
        public string Id { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("E-mail")]
        public string Email { get; set; }

        [DisplayName("Create Date")]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Enabled")]
        public bool Enabled { get; set; }

        public UserCredentials UserCredentials { get; private set; }
        public List<string> Subscriptions { get; private set; }
        public List<Address> Addresses { get; private set; }
        public UserMigrationInfo UserMigrationInfo { get; set; }
        public string RedeemedPromotion { get; set; }
        public List<string> PaymentProviderIds { get; set; }

        public string PaymentProviderId { get; set; }
        public List<dynamic> Readers { get; set; }
            
        public User AddCredentials(UserCredentials credentials)
        {
            UserCredentials = credentials;
            return this;
        }

        public User AddAddress(Address address, bool useAsBothShippingAndBilling)
        {
            if(useAsBothShippingAndBilling)
            {
                var shippingAddress = address.Copy();
                shippingAddress.AddressType = AddressType.Shipping.ToString();
                AddAddress(shippingAddress);

                var billingingAddress = address.Copy();
                billingingAddress.AddressType = AddressType.Billing.ToString();
                return AddAddress(billingingAddress);
            }

            return AddAddress(address);
        }

        public User AddAddress(Address address)
        {
            Addresses.Add(address);
            return this;
        }

        public User AddMigrationInfo(UserMigrationInfo userMigrationInfo)
        {
            UserMigrationInfo = userMigrationInfo;
            return this;
        }

        public User AddAddress(List<Address> addresses)
        {
            Addresses.AddRange(addresses);
            return this;
        }

        [DisplayName("Friendly ID")]
        public string FriendlyId { get { return Id.Substring(Id.IndexOf("/") + 1); } }

        public bool HasAccountInfo
        {
            get { return Addresses.Count >= 1; }
        }

        [DisplayName("Is Admin")]
        public bool IsAdmin { get; set; }

        public Address BillingAddress
        {
            get { return Addresses.FirstOrDefault(address => address.AddressType == AddressType.Billing.ToString()); }
        }

        public Address ShippingAddress
        {
            get { return Addresses.FirstOrDefault(address => address.AddressType == AddressType.Shipping.ToString()); }
        }
    }
}