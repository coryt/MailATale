using System;
using Castle.Core.Logging;
using MAT.Core.InputModels;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;
using MAT.Web.Infrastructure.Common;
using Raven.Client;

namespace MAT.Web.Helpers.Builders
{
    public class UserBuilder
    {
        private readonly IDocumentSession _ravenSession;
        private readonly ILogger _logger;

        public UserBuilder(IDocumentSession ravenSession, ILogger logger)
        {
            _ravenSession = ravenSession;
            _logger = logger;
        }

        public bool TryValidateAccountInfo(RegisterAccountModel account, out User user)
        {
            user = null;
            try
            {
                var credentials =  new UserCredentials(account.Email).SetPassword(account.Password);
                var street2 = account.Street2 ?? string.Empty;
                var phone = account.Phone ?? string.Empty;
                var address = new Address(AddressType.Shipping,
                                          account.Street1,
                                          street2,
                                          account.City,
                                          Enumeration.FromValueOrDefault(typeof(Province), account.Province).DisplayName,
                                          account.PostalCode,
                                          phone);

                user = new User(account.FirstName, account.LastName, account.Email, true)
                       .AddAddress(address, true)
                       .AddCredentials(credentials);

                var existing = _ravenSession.GetUserByCredentialId(user.UserCredentials.Email);
                if (existing != null)
                    return false;

                _ravenSession.Store(user);

                return true;
            }
            catch (Exception e)
            {
                _logger.Fatal("There was a problem with the account info.", e);
                return false;
            }
        }
    }
}