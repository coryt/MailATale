using System.Collections.Generic;
using System.Linq;
using MAT.Core.Models.Account;

namespace MAT.Core.InputModels
{
    public class PreferenceQuestionAnswerModel
    {
        public PreferenceQuestionAnswerModel()
        {
        }

        public PreferenceQuestionAnswerModel(string id)
        {
            QuestionId = id;
        }

        public string QuestionId { get; set; }
        public string Answer { get; set; }

        public static List<PreferenceQuestionAnswerModel> ListFor(IEnumerable<PreferenceQuestion> questions)
        {
            if (questions == null) return new List<PreferenceQuestionAnswerModel>();
            return questions.Select(q => new PreferenceQuestionAnswerModel(q.Id)).ToList();
        }
    }
}