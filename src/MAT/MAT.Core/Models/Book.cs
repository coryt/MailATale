using MAT.Core.Models.Enumerations;

namespace MAT.Core.Models
{
    public class Book
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AgeGroups AgeGroup { get; set; }
        public int Stock { get; set; }
        public BookThemes Theme { get; set; }
    }
}