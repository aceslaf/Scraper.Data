using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scraper.Data.Common
{
    public static class StringExstensions
    {
        public static string ToColumnName(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            var str = rgx.Replace(s, "");
            if (str == null) {
                return string.Empty;
            }
            return str.SafeReplace(" ", "");
        }


        public static string SafeReplace(this string s, string oldVal, string newVal)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(oldVal))
            {
                return s.Replace(oldVal, newVal == null ? string.Empty: newVal);
            }
            return s;
        }

    }
}
