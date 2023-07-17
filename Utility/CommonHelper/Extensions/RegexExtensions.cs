using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonHelper
{
    public static class RegexExtensions
    {
        public static string RegexReplace(this string originalValue, string pattern, string replaceString)
        {
            Regex regex = new Regex(pattern);

            return regex.Replace(originalValue, replaceString);
        }

        public static bool RegexMatch(this string targetValue, string pattern)
        {
            Match match = Regex.Match(targetValue, pattern);

            return match.Success;
        }
        public static string GetRegexMatch(this string targetValue, string pattern)
        {
            Match match = Regex.Match(targetValue, pattern);
            return match.Success ? match.Value.Trim() : targetValue;
        }

        /// <summary>
        /// 判斷是否為英數字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isNaturalNumber(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
            return reg.IsMatch(str);
        }

        /// <summary>
        /// 判斷是否為英數字,與底線
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isNaturalNumberDash(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9_]+$");
            return reg.IsMatch(str);
        }
        /// <summary>
        /// 判斷是否為數字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isNumber(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[0-9]+$");
            return reg.IsMatch(str);
        }

        /// <summary>
        /// 判斷是否為email格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsVaildEmail(string str)
        {
            str = str.TrimEnd(' ');
            string removeSpace = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "");
            if (removeSpace.Length != str.Length)
            {
                return false;
            }//空白存在

            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"(@)(.+)$");
            return reg.IsMatch(str);
        }
    }
}
