using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace MAT.Core.Models.Account
{
    public class UserCredentials
    {
        public string Email { get; set; }
        public HashVersions HashVersion { get; private set; }

        public UserCredentials()
        {
            _hashFunctions = new Dictionary<HashVersions, Func<string, string, string>> { { HashVersions.FormsAuth, GetFormsAuthHash }, { HashVersions.Custom, GetCustomHash } };
        }

        public UserCredentials(string email) :this()
        {
            Email = email;
        }

        public enum HashVersions
        {
            FormsAuth = 0,
            Custom = 1
        }

        const string ConstantSalt = "xi07cevs01q4#";
        protected string HashedPassword { get; private set; }
        private string _passwordSalt;
        private string PasswordSalt
        {
            get
            {
                return _passwordSalt ?? (_passwordSalt = Guid.NewGuid().ToString("N"));
            }
            set { _passwordSalt = value; }
        }

        private readonly Dictionary<HashVersions, Func<string, string, string>> _hashFunctions;
        public UserCredentials SetPassword(string pwd)
        {
            HashVersion = HashVersions.Custom;
            HashedPassword = _hashFunctions[HashVersions.Custom](PasswordSalt, pwd);
            return this;
        }

        public void SetHashedPasswordFromExistingPassword(string hash, DateTime dateSalt)
        {
            HashVersion = HashVersions.FormsAuth;
            HashedPassword = hash;
            PasswordSalt = dateSalt.ToString("dd/MM/yyyyh:mm:00tt", CultureInfo.InvariantCulture);
        }

        public bool ValidatePassword(string maybePwd)
        {
            if (HashedPassword == null)
                return true;
            return HashedPassword == _hashFunctions[HashVersion](PasswordSalt, maybePwd);
        }

        private string GetCustomHash(string salt, string password)
        {
            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(Encoding.Unicode.GetBytes(salt + password + ConstantSalt));
                return Convert.ToBase64String(computedHash);
            }
        }

        private string GetFormsAuthHash(string salt, string password)
        {
            var saltedPassword = string.Format("{0}{1}", salt, password);
            return FormsAuthentication.HashPasswordForStoringInConfigFile(saltedPassword, "SHA1");
        }
    }
}