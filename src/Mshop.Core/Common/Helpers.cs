using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mshop.Core.Common
{
    public static class Helpers
    {
        public static string ClearString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            string pattern = @"[-/_.,()\*\$#&@[\]{}""%+\\]";
            return Regex.Replace(value, pattern, string.Empty);
        }
    }
}
