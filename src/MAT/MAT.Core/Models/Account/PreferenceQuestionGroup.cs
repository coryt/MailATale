using System.Collections.Generic;
using MAT.Core.Models.Enumerations;

namespace MAT.Core.Models.Account
{
    public class PreferenceQuestionGroup
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public Dictionary<AgeGroups, PreferenceQuestion> Questions { get; set; }
    }
}