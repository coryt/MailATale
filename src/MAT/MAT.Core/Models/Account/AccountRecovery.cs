using System;

namespace MAT.Core.Models.Account
{
    public class AccountRecovery
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
       
        public AccountRecovery(string email, string token)
        {
            Email = email;
            Token = token;
            ExpiresOn = DateTime.Now.AddHours(1);
        }
    }
}