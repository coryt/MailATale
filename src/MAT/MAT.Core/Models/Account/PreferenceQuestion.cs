using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MAT.Core.Models.Enumerations;

namespace MAT.Core.Models.Account
{
    public class PreferenceQuestion   
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public string StorySentance { get; set; }
        public List<string> Answers { get; set; }
        public bool MultiResponse { get; set; }
        public bool Active { get; set; }
        public bool IncludeInSignup { get; set; }
        public string Category { get; set; }
        public AgeGroups AgeGroup { get; set; }
        public string Width { get; set; }

        public SelectList GetAnswerList()
        {
            if(Answers == null)
                return new SelectList(Enumerable.Empty<SelectListItem>());
            var answers = Answers.Select(a => new SelectListItem {Text = a, Value = a});
            return new SelectList(answers, "Text", "Value", null);
        }
    }
}