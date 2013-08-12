using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace MAT.Core.Extensions
{
    public static class EnumExtensions
    {
        public static SelectList ToSelectList<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) return null;

            var values = from T enumValue in Enum.GetValues(typeof(T))
                         let name = Regex.Replace(enumValue.ToString(), "(\\B[A-Z])", " $1")
                         select new { DisplayName = name, Value = enumValue };

            return new SelectList(values, "Value", "DisplayName");
        }
    }
}
