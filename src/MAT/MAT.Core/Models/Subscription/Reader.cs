using System;
using System.Collections.Generic;
using System.Linq;
using MAT.Core.InputModels;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;

namespace MAT.Core.Models.Subscription
{
    public class Reader
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string BookPreference { get; set; }
        public string ReadingLevel { get; set; }
        public string ReadingStyle { get; set; }
        public string RelationshipToUser { get; set; }
        public DateTime Birthdate { get; set; }
        public string Grade { get; set; }
        public List<KeyValuePair<PreferenceQuestion, string>> PreferenceAnswers { get; set; }

        public Reader()
        {
            PreferenceAnswers = new List<KeyValuePair<PreferenceQuestion, string>>();
        }

        public AgeGroups GetAgeGroup()
        {
            //find age of reader
            DateTime now = DateTime.Today;
            int age = now.Year - Birthdate.Year;
            if (Birthdate > now.AddYears(-age)) age--;

            //determind age group
            var ageGroup = AgeGroups.AgeEightPlus;
            if (age >= 0 && age < 4)
            {
                ageGroup = AgeGroups.AgeZeroThree;
            }
            else if (age >= 4 && age < 8)
            {
                ageGroup = AgeGroups.AgeFourSeven;
            }

            return ageGroup;
        }

        public void AddPreferenceQuestion(PreferenceQuestion question, string answer)
        {
            PreferenceAnswers.Add(new KeyValuePair<PreferenceQuestion, string>(question, answer));
        }

        public Reader SetFromModel(ReaderModel readerModel)
        {
            Birthdate = DateTime.Parse(readerModel.Dob);
            Gender = Enumeration.FromValueOrDefault(typeof (Gender), Convert.ToInt32(readerModel.Gender)).DisplayName;
            Name = readerModel.Name;
            Grade = Enumeration.FromValueOrDefault(typeof (GradeLevel), Convert.ToInt32(readerModel.GradeLevel)).DisplayName;
            ReadingLevel = Enumeration.FromValueOrDefault(typeof (ReadingLevel), Convert.ToInt32(readerModel.ReadingLevel)).DisplayName;
            RelationshipToUser = Enumeration.FromValueOrDefault(typeof (Relationships),Convert.ToInt32(readerModel.Relationship)).DisplayName;

            return this;
        }

        public void AddPreferenceResponse(List<PreferenceQuestion> signupQuestions, List<PreferenceQuestionAnswerModel> preferenceQuestionAnswer)
        {
            foreach (var answerModel in preferenceQuestionAnswer)
            {
                var model = answerModel;
                var question = signupQuestions.FirstOrDefault(q => q.Id == model.QuestionId);
                if (question == null) continue;
                PreferenceAnswers.Add(new KeyValuePair<PreferenceQuestion, string>(question, answerModel.Answer));
            }
        }
    }
}