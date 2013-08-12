using System.Collections.Generic;
using System.Web.Mvc;

namespace MAT.Core.Models.Enumerations
{
    public class GradeLevel : Enumeration
    {
        public static readonly GradeLevel PreKindergarten = new GradeLevel(0, "Pre-Kindergarten");
        public static readonly GradeLevel JrKindergarten = new GradeLevel(1, "Jr. Kindergarten");
        public static readonly GradeLevel SrKindergarten = new GradeLevel(2, "Sr. Kindergarten");
        public static readonly GradeLevel One = new GradeLevel(3, "1");
        public static readonly GradeLevel Two = new GradeLevel(4, "2");
        public static readonly GradeLevel Three = new GradeLevel(5, "3");
        public static readonly GradeLevel Four = new GradeLevel(6, "4");
        public static readonly GradeLevel Five = new GradeLevel(7, "5");
        public static readonly GradeLevel SixPlus = new GradeLevel(8, "6+");
        public static readonly GradeLevel NotInSchool = new GradeLevel(9, "Not in school"); 

        private GradeLevel() { }
        private GradeLevel(int value, string displayName) : base(value, displayName) { }

        public static SelectList GetGradeLevels()
        {
            var grades = new List<GradeLevel>() 
                { 
                    NotInSchool,
                    PreKindergarten, 
                    JrKindergarten, 
                    SrKindergarten, 
                    One, 
                    Two, 
                    Three, 
                    Four, 
                    Five, 
                    SixPlus, 
                };

            return new SelectList(grades, "Value", "DisplayName", null);
        }
    }
}
