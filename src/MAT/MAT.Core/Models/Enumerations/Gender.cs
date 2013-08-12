using System.Web.Mvc;

namespace MAT.Core.Models.Enumerations
{
    public class Gender : Enumeration
    {
        public static readonly Gender Female = new Gender(0, "Female");
        public static readonly Gender Male = new Gender(1, "Male");

        private Gender() { }
        private Gender(int value, string displayName) : base(value, displayName) { }

        public static SelectList GetGenderSelectList()
        {
            return new SelectList(new[] { Female, Male }, "Value", "DisplayName", null);
        }
    }
}