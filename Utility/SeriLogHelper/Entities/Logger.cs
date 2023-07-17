using Microsoft.Extensions.Logging;

namespace SeriLogHelper.Entities
{
    public class Logger
    {
        /// <summary>
        /// 記錄使用的日誌記錄器名稱
        /// </summary>
        public string LoggerName { get; set; }
        /// <summary>
        /// 記錄應用運行的環境，例如 dev、test、prod 等。
        /// </summary>
        public string Environment { get; set; }
        /// <summary>
        /// 記錄應用名稱，用於區分不同的應用。
        /// </summary>
        public string Application { get; set; }
        /// <summary>
        /// 記錄應用所在的服務器名稱。
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// 記錄日誌的級別，包括 Debug、Info、Warning、Error、Fatal 等。
        /// </summary>
        public LogLevel LogLevel { get; set; }
        /// <summary>
        /// 記錄日誌信息的內容。
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 記錄異常信息。
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 記錄日誌記錄時間。
        /// </summary>
        public DateTime TimeStamp { get; set; }
        /// <summary>
        /// 記錄相關性 ID，用於關聯同一交易中的不同日誌。
        /// </summary>
        public string CorrelationId { get; set; }
        /// <summary>
        /// 記錄交易 ID，用於關聯同一交易中的不同日誌。
        /// </summary>
        public string TransactionId { get; set; }
        /// <summary>
        /// 記錄操作名稱，例如訪問某個 API、調用某個方法等。
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// 記錄用戶名稱。
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 記錄用戶 ID。
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 記錄訪問的 IP 地址。
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 記錄應用所在的機器名稱。
        /// </summary>
        public string MachineName { get; set; }
        /// <summary>
        /// 記錄產生日誌的程式集名稱。
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        /// 記錄產生日誌的類名稱。
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 記錄產生日誌的方法名稱。
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 記錄產生日誌時傳入的參數。
        /// </summary>
        public string Parameters { get; set; }
        /// <summary>
        /// 記錄訪問的 API
        /// </summary>
        public string RequestPath { get; set; }
        /// <summary>
        /// 記錄了 API 請求所使用的 HTTP 方法，例如 GET、POST、PUT、DELETE 等。
        /// </summary>
        public string RequestMethod { get; set; }
        /// <summary>
        /// 記錄了 API 請求的 Content-Type，通常是指請求主體資料的格式，例如 application/json、application/xml 等。
        /// </summary>
        public string RequestContentType { get; set; }
        /// <summary>
        /// 記錄了 API 回應的 Content-Type，通常是指回應主體資料的格式，例如 application/json、application/xml 等。
        /// </summary>
        public string ResponseContentType { get; set; }
        /// <summary>
        /// 記錄了 API 請求的主體內容，通常是一段 JSON 或 XML 格式的文字。
        /// </summary>
        public string RequestBody { get; set; }
        /// <summary>
        /// 記錄了 API 回應的主體內容，通常是一段 JSON 或 XML 格式的文字。
        /// </summary>
        public string ResponseBody { get; set; }
        /// <summary>
        /// 記錄了 API 請求所花費的時間，通常是以毫秒為單位的數字，例如 1234 毫秒。
        /// </summary>
        public long ElapsedMilliseconds { get; set; }
    }
}
