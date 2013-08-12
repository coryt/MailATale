using System.Collections.Generic;
using System.Web.Mvc;

namespace MAT.Core.Models.Enumerations
{
    public class Relationships : Enumeration
    {
        public static readonly Relationships Mother = new Relationships(0, "Mother");
        public static readonly Relationships Father = new Relationships(1, "Father");
        public static readonly Relationships Sibling = new Relationships(2, "Sibling");
        public static readonly Relationships Uncle = new Relationships(3, "Uncle");
        public static readonly Relationships Aunt = new Relationships(4, "Aunt");
        public static readonly Relationships GrandParent = new Relationships(5, "GrandParent");
        public static readonly Relationships Friend = new Relationships(6, "Friend");
        public static readonly Relationships Other = new Relationships(7, "Other");

        private Relationships() { }
        private Relationships(int value, string displayName) : base(value, displayName) { }

        public static SelectList GetRelationshipList()
        {
            var relationships = new List<Relationships>()
                {
                    Mother,
                    Father,
                    Sibling,
                    Uncle,
                    Aunt,
                    GrandParent,
                    Friend,
                    Other
                };

            return new SelectList(relationships, "Value", "DisplayName", null);
        }
    }
}