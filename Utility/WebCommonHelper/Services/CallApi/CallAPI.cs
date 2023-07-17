using CommonHelper;
using EncryptionHelper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog.Fluent;
using System.Dynamic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebCommonHelper.Config;

namespace WebCommonHelper.Services.CallApi
{
    public class CallAPI : IConnect
    {
        private string? jwtToken { get; set; }
        private string sessionId { get; set; } = "";

        private readonly APISettings apiSettings;

        private class ResponseObj
        {
            public string message { get; set; }
        }

        public CallAPI(IOptions<APISettings> apiSettings)
        {
            this.apiSettings = apiSettings.Value;
        }

        /// <summary>
        /// 傳入Token
        /// </summary>
        /// <param name="_Jwt"></param>
        public void SetJwt(string jwtToken)
        {
            this.jwtToken = jwtToken;
        }

        /// <summary>
        /// 傳入SessionId
        /// </summary>
        /// <param name="_SessionId"></param>
        public void SetSessionId(string sessionId)
        {
            this.sessionId = sessionId;
        }

        /// <summary>
        /// Http Post
        /// </summary>
        /// <typeparam name="T">傳入參數物件泛型</typeparam>
        /// <param name="PostParameter">傳入參數物件</param>
        /// <param name="ControllerName">目標Controller 名稱</param>
        /// <returns></returns>
        public async Task<string> Post<T>(T? PostParameter, string ControllerName, bool autoToken = false)
        {
            if (autoToken && string.IsNullOrEmpty(jwtToken))
            {
                string? newToken = await GetToken();
                SetJwt(newToken);
            }

            return await PostAsync<T>(PostParameter, ControllerName, "Post");
        }

        /// <summary>
        /// Http Get 
        /// </summary>
        /// <param name="PostParameter">傳入參數字串</param>
        /// <param name="ControllerName">目標Controller 名稱</param>
        /// <returns></returns>
        public async Task<string> Get(string PostParameter, string ControllerName, bool autoToken = false)
        {
            if (autoToken && string.IsNullOrEmpty(jwtToken))
            {
                string? newToken = await GetToken();
                SetJwt(newToken);
            }

            return await PostAsync<string>(PostParameter, ControllerName, "Get");
        }

        /// <summary>
        /// 實作 connect to api and send httprequest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="postParameter"></param>
        /// <param name="controllerName"></param>
        /// <param name="sendMethod"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<string> PostAsync<T>(T? postParameter, string controllerName, string sendMethod = "Post")
        {
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    HttpResponseMessage response = null;

                    try
                    {
                        #region  設定相關網址內容
                        // Accept 用於宣告客戶端要求服務端回應的文件型態 (底下兩種方法皆可任選其一來使用)
                        client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

                        if (!string.IsNullOrEmpty(jwtToken))
                        {
                            client.DefaultRequestHeaders.Add("Authorization", $@"Bearer {jwtToken}");
                        }

                        string url = apiSettings.Api.ip.EndsWith(@"/") ? $@"{apiSettings.Api.ip}{controllerName}" : $@"{apiSettings.Api.ip}/{controllerName}";

                        string postContent = "";
                        if (postParameter != null && postParameter.ToString() != "")
                        {
                            if (sendMethod.ToUpper() == "POST")
                            {
                                if (!string.IsNullOrEmpty(sessionId))
                                {
                                    dynamic dynamicPostParameter = CreateExpandoObject(postParameter);
                                    postContent = apiSettings.Api.doEncryptAndDecrypt
                                                    ? EncryptionService.AesEncrypt(JsonConvert.SerializeObject(dynamicPostParameter), apiSettings.Api.key, apiSettings.Api.salt)
                                                    : System.Text.Json.JsonSerializer.Serialize(dynamicPostParameter);
                                }
                                else
                                {
                                    postContent = apiSettings.Api.doEncryptAndDecrypt
                                                    ? EncryptionService.AesEncrypt(JsonConvert.SerializeObject(postParameter), apiSettings.Api.key, apiSettings.Api.salt)
                                                    : System.Text.Json.JsonSerializer.Serialize(postParameter);
                                }
                            }
                            else
                            {
                                postContent = apiSettings.Api.doEncryptAndDecrypt
                                                ? EncryptionService.AesEncrypt(postParameter.ToString(), apiSettings.Api.key, apiSettings.Api.salt)
                                                : postParameter.ToString();
                            }
                        }
                        #endregion

                        #region 呼叫遠端 Web API 
                        if (sendMethod.ToUpper() == "POST")
                        {
                            using (var fooContent = new StringContent(postContent, Encoding.UTF8, "application/json"))
                            {
                                response = await client.PostAsync(url, fooContent);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(postContent))
                            {
                                url = url.EndsWith(@"/") ? url.Remove(url.Length - 1) : url;
                                url += "?Parameters=" + postContent;
                            }
                            response = await client.GetAsync(url);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        throw new RequestApiException("DoApiJsonAsync Error : " + ex.ToString());
                    }

                    #region 處理呼叫完成 Web API 之後的回報結果
                    if (response != null)
                    {
                        return await ResponseHandle(response);
                    }
                    else
                    {
                        throw new RequestApiException("DoApiJsonAsync Error : response is null ");
                    }
                    #endregion
                }
            }
        }

        public async Task<Tuple<bool, string>> PostSOAPXML(string URL, string PostContent)
        {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(PostContent);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentLength = bs.Length;
            request.ContentType = "application/soap+xml; charset=utf-8";
            request.Timeout = 300000;

            try
            {
                using (Stream DataStream = request.GetRequestStream())
                {
                    DataStream.Write(bs, 0, bs.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    // Grab the JSON response as a string
                    string rawXML = reader.ReadToEnd();
                    return new Tuple<bool, string>(true, rawXML.ToSafeString());
                }
            }
            catch (WebException we)
            {
                string errorMSG = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                return new Tuple<bool, string>(false, errorMSG);
            }
            catch (Exception ex)
            {
                string errorMSG = JsonConvert.SerializeObject(ex);
                return new Tuple<bool, string>(false, errorMSG);
            }
        }

        private async Task<string> ResponseHandle(HttpResponseMessage response)
        {
            string responseFormServer = response.Content.ReadAsStringAsync().Result;
            if (apiSettings.Api.doEncryptAndDecrypt)
            {
                responseFormServer = EncryptionService.AesDecrypt(responseFormServer, apiSettings.Api.key, apiSettings.Api.salt);
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return responseFormServer;
                case HttpStatusCode.BadRequest:
                    throw new WebCommonHelperException(responseFormServer);
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedAccessException(responseFormServer);
                case HttpStatusCode.NotFound:
                    throw new KeyNotFoundException(responseFormServer);
                case HttpStatusCode.RequestTimeout:
                    throw new TokenTimeoutException(responseFormServer);
                case HttpStatusCode.Conflict:
                    throw new LogicErrorException(responseFormServer);
                default:
                    int statusCode = (int)response.StatusCode;
                    var statusCodeName = response.StatusCode.ToString();
                    throw new RequestApiException($@"{statusCodeName}({statusCode}):{responseFormServer}");
            }
        }

        private ExpandoObject CreateExpandoObject(object source)
        {
            var result = new ExpandoObject();
            bool SessionIdBool = false;
            IDictionary<string, object> dynamicPostParameter = result;

            foreach (var property in source.GetType().GetProperties().Where(p => p.CanRead && p.GetMethod.IsPublic))
            {
                dynamicPostParameter[property.Name] = property.GetValue(source, null);
                if (property.Name == "SessionId")
                {
                    SessionIdBool = true;
                }
            }
            if (SessionIdBool == false)
            {
                dynamicPostParameter["SessionId"] = sessionId;
            }

            return result;
        }

        private async Task<string?> GetToken()
        {
            string response = await Get("", "Token");
            JsonObject returnData = response.Deserialize<JsonObject>();

            return returnData
                .Validate("Token", "Something went error, please contact Computer User Help Desk.")
                .NotNullOrEmpty().MinimumLength(1)
                .GetValue<string>();
        }
    }
}
