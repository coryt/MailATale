using System.Linq;
using MAT.Core.Models;
using MAT.Core.Models.Account;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace MAT.Web.Infrastructure.Indexes
{
    public class PreferenceQuestions_ByIncludeInSignup : AbstractIndexCreationTask<PreferenceQuestion>
    {
        public PreferenceQuestions_ByIncludeInSignup()
        {
            Map = preferenceQuestions => from question in preferenceQuestions
                                         select new { question.IncludeInSignup };
            Index(x => x.IncludeInSignup, FieldIndexing.Default);
        }
    }
}