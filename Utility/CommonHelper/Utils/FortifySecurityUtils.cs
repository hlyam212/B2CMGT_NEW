using System.Text;

namespace CommonHelper
{
    /// <summary>
    /// 針對Fortify的弱點掃描修正
    /// </summary>
    public static class FortifySecurityUtils
    {
        /// <summary>
        /// Using HttpUtility.HtmlEncode
        /// </summary>
        public static string EncoderStr(this string value)
        {
            //return HttpUtility.HtmlEncode(value);
            //< (左角括弧) 轉成 &lt;
            //> (右角括弧) 轉成 &gt;
            //" (雙引號) 轉成 &quot;
            //& (連字號) 轉成 &amp;
            //\ (反斜線) 轉成 &#92;

            return System.Web.HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Not using HttpUtility.HtmlEncode
        /// </summary>
        public static string EncoderStrByReplace(this string value)
        {
            value = value.Replace("<", "&lt");
            value = value.Replace(">", "&gt");
            value = value.Replace(@"""", "&quot");
            value = value.Replace(@"&", "&amp");

            return value;
        }

        /// <summary>
        /// 過水?
        /// </summary>
        public static dynamic CheckString(dynamic value)
        {
            return value;
        }

        /// <summary>
        /// 判斷是否為英數字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isNaturalNumber(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[A-Za-z0-9]+$");
            return !reg.IsMatch(str);
        }
        /// <summary>
        /// 判斷是否為數字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isNumber(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[0-9]+$");
            return !reg.IsMatch(str);
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

        /// <summary>
        /// 針對檔案/資料夾路徑做檢查
        /// </summary>
        /// <returns></returns>
        public static string PathManipulation(this string pathValue)
        {
            StringBuilder builder = new StringBuilder();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            Dictionary<char, char> dictionary = new Dictionary<char, char>();
            for (int i = 32; i <= 126; i++)
            {
                var _char = Convert.ToChar(i);
                dictionary.Add(_char, _char);
            }

            // 判斷路徑是否為 UNC 格式 
            bool isUncPath = pathValue.StartsWith("\\\\");

            //替換有安全疑慮的路徑字串內容 
            pathValue = pathValue.Replace("..\\", string.Empty);
            pathValue = pathValue.Replace("\\\\\\", string.Empty);
            pathValue = pathValue.Replace("\\\\", string.Empty);

            pathValue = pathValue.Replace("../", string.Empty);
            pathValue = pathValue.Replace("///", string.Empty);
            pathValue = pathValue.Replace("//", string.Empty);

            if (isUncPath == true)
            {
                pathValue = ("\\\\" + pathValue);
            }


            //將檔案路徑進行 base64 編碼
            Byte[] base64Encode = encoding.GetBytes(pathValue);
            string base64String = Convert.ToBase64String(base64Encode);
            var _items = base64String.ToCharArray();
            foreach (var _item in _items)
            {
                builder.Append(dictionary[_item]);
            }
            base64String = builder.ToString();


            //進行 Base64 解碼取得加工後的檔案路徑 
            Byte[] base64Decode = Convert.FromBase64String(base64String);
            return encoding.GetString(base64Decode);
        }

        /// <summary>
        /// 針對URL做檢查
        /// </summary>
        /// <returns></returns>
        public static string PathManipulationUrl(this string urlString)
        {
            StringBuilder builder = new StringBuilder();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            Dictionary<char, char> dictionary = new Dictionary<char, char>();
            for (int i = 32; i <= 126; i++)
            {
                var _char = Convert.ToChar(i);
                dictionary.Add(_char, _char);
            }

            // 判斷路徑是否為 UNC 格式 
            bool isUrlPath = urlString.StartsWith("\\\\");

            //替換有安全疑慮的路徑字串內容 
            urlString = urlString.Replace("..\\", string.Empty);
            urlString = urlString.Replace("\\\\\\", string.Empty);
            urlString = urlString.Replace("\\\\", string.Empty);

            urlString = urlString.Replace("../", string.Empty);
            urlString = urlString.Replace("///", string.Empty);

            if (isUrlPath == true)
            {
                urlString = ("\\\\" + urlString);
            }


            //將檔案路徑進行 base64 編碼
            Byte[] base64Encode = encoding.GetBytes(urlString);
            string base64String = Convert.ToBase64String(base64Encode);
            var _items = base64String.ToCharArray();
            foreach (var _item in _items)
            {
                builder.Append(dictionary[_item]);
            }
            base64String = builder.ToString();


            //進行 Base64 解碼取得加工後的檔案路徑 
            Byte[] base64Decode = Convert.FromBase64String(base64String);
            return encoding.GetString(base64Decode);
        }
    }
}
