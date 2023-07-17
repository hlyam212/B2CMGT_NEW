using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommonHelper
{
    /// <summary>
    /// custom exception class for throwing application apecific exception (e.g. for validation)
    /// that can be caught and handled within the application
    /// </summary>
    public class WebCommonHelperException : Exception
    {
        public WebCommonHelperException() : base() { }
        public WebCommonHelperException(string message) : base(message) { }
        public WebCommonHelperException(string message, Exception inner) : base(message, inner) { }
        public WebCommonHelperException(string message, params object[] args) :base(String.Format(CultureInfo.CurrentCulture, message, args))
        {

        }
    }

    /// <summary>
    /// 設定檔不完整
    /// </summary>
    public class OptionMissingException : Exception
    {
        public OptionMissingException() : base() { }
        public OptionMissingException(string message) : base(message) { }
        public OptionMissingException(string message, Exception inner) : base(message, inner) { }
        public OptionMissingException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {

        }
    }

    /// <summary>
    /// 邏輯錯誤
    /// </summary>
    public class LogicErrorException : Exception
    {
        public LogicErrorException() : base() { }
        public LogicErrorException(string message) : base(message) { }
        public LogicErrorException(string message, Exception inner) : base(message, inner) { }
        public LogicErrorException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {

        }
    }

    /// <summary>
    /// 邏輯錯誤
    /// </summary>
    public class TokenTimeoutException : Exception
    {
        public TokenTimeoutException() : base() { }
        public TokenTimeoutException(string message) : base(message) { }
        public TokenTimeoutException(string message, Exception inner) : base(message, inner) { }
        public TokenTimeoutException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {

        }
    }

    /// <summary>
    /// API端的錯誤
    /// </summary>
    public class RequestApiException : Exception
    {
        public RequestApiException() : base() { }
        public RequestApiException(string message) : base(message) { }
        public RequestApiException(string message, Exception inner) : base(message, inner) { }
        public RequestApiException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {

        }
    }
}
