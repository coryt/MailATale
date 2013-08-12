using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MAT.Core.Models.Account;
using MAT.Core.Models.Subscription;

namespace MAT.Core.InputModels
{
    public class ReaderModel
    {
        public ReaderModel()
        {
            PreferenceAnswers = new List<PreferenceQuestionAnswerModel>();
        }

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        public string Dob { get; set; }

        [Required]
        public string GradeLevel { get; set; }

        [Required]
        public string ReadingLevel { get; set; }

        [Required]
        public string Relationship { get; set; }

        [Required]
        [Range(1, 31)]
        [Display(Name = "Day")]
        public int? DobDay { get; set; }

        [Required]
        [Range(1, 12)]
        [Display(Name = "Month")]
        public int? DobMonth { get; set; }

        [Required]
        [Display(Name = "Year")]
        public int? DobYear { get; set; }

        [Required]
        public List<PreferenceQuestionAnswerModel> PreferenceAnswers { get; set; }

        public Reader ToReader(List<PreferenceQuestion> signupQuestions)
        {
            var reader = new Reader()
                 {
                     Birthdate = GetBirthdate(),
                     Gender = Gender,
                     ReadingLevel = ReadingLevel,
                     RelationshipToUser = Relationship,
                     Grade = GradeLevel,
                     Name = Name,
                 };

            reader.AddPreferenceResponse(signupQuestions, PreferenceAnswers);

            return reader;
        }

        public DateTime GetBirthdate()
        {
            int year, month, day;

            if (DobDay == null || DobMonth == null || DobYear == null)
            {
                const string pattern = @"\s*(\d+)/(\d+)/(\d+)\s*";
                var match = Regex.Match(Dob, pattern);
                if (!match.Success || match.Captures.Count != 4)
                    throw new InvalidOperationException("Invalid DOB");
                year = int.Parse(match.Captures[3].Value);
                month = int.Parse(match.Captures[1].Value);
                day = int.Parse(match.Captures[2].Value);
            }
            else
            {
                year = DobYear.Value;
                month = DobMonth.Value;
                day = DobDay.Value;
            }

            return new DateTime(year, month, day);
        }
    }
}