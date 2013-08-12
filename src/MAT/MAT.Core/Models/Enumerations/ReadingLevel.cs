using System.Collections.Generic;
using System.Web.Mvc;

namespace MAT.Core.Models.Enumerations
{
    public class ReadingLevel : Enumeration
    {
        public static readonly ReadingLevel AtAgeLevel = new ReadingLevel(0, "at age level");
        public static readonly ReadingLevel AboveAgeLevel = new ReadingLevel(1, "above age level");
        public static readonly ReadingLevel ChewingBooks = new ReadingLevel(2, "chewing on books");

        private ReadingLevel() { }
        private ReadingLevel(int value, string displayName) : base(value, displayName) {}
        
        public static SelectList GetReadingLevels()
        {
            var grades = new List<ReadingLevel>() 
                { 
                    AtAgeLevel, 
                    AboveAgeLevel, 
                    ChewingBooks, 
                };

            return new SelectList(grades, "Value", "DisplayName", null);
        } 
    }
}