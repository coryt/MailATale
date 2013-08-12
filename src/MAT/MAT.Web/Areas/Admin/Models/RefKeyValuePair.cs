using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MAT.Web.Areas.Admin.Models
{
    public class RefKeyValuePair<TKey, TValue>
    {
        public RefKeyValuePair(KeyValuePair<TKey, TValue> pair)
        {
            Key = pair.Key;
            Value = pair.Value;
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }
}