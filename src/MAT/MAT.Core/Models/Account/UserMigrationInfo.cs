using System;
using System.Collections.Generic;
using MAT.Core.Models.Subscription;

namespace MAT.Core.Models.Account
{
    public class UserMigrationInfo  
    {
        public UserMigrationInfo()
        {
            PackageMigrationInfo = new List<PackageMigrationInfo>();
        }

        public string Id { get; set; }
        public DateTime MigratedOn { get; set; }
        public List<PackageMigrationInfo> PackageMigrationInfo { get; set; }
    }
}