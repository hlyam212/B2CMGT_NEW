using CommonHelper;
using EncryptionHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using WebCommonHelper.Config;

namespace WebCommonHelper.Middlewares
{

    /// <summary>
    /// 資料讀取解密
    /// </summary>
    public class DecryptMiddleware
    {
        private readonly RequestDelegate next;
        private readonly APISettings apiSettings;

        public DecryptMiddleware(RequestDelegate _next, IOptions<APISettings> _apiSettings)
        {
            next = _next;
            apiSettings = _apiSettings.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (context.Request.Method == "GET")
                {
                    var request = context.Request;
                    string url = context.Request.GetDisplayUrl();
                    if (url.Contains(@"?Parameters="))
                    {
                        List<string> urlList = url.Split(@"?Parameters=").ToList();
                        if (urlList.Count == 2)
                        {
                            //解密
                            string decriptedPostParameter = EncryptionService.AesDecrypt(urlList[1], apiSettings.Api.key, apiSettings.Api.salt);
                            NameValueCollection nc = System.Web.HttpUtility.ParseQueryString(decriptedPostParameter);
                            context.Request.QueryString = new QueryString();
                            foreach (string nc_name in nc)
                            {
                                context.Request.QueryString = context.Request.QueryString.Add(nc_name, nc[nc_name].ToString());
                            }
                        }
                    }
                }//Get
                else
                {
                    var request = context.Request;
                    request.EnableBuffering();
                    using (var reader = new StreamReader(request.Body))
                    {
                        //取得值
                        var PostParameter = await Task.Run(reader.ReadToEndAsync);

                        //解密
                        var decriptedStr = EncryptionService.AesDecrypt(PostParameter, apiSettings.Api.key, apiSettings.Api.salt);
                        //回寫body
                        byte[] bytes = Encoding.ASCII.GetBytes(decriptedStr);
                        request.Body = new MemoryStream(bytes);

                    }
                    request.Body.Position = 0;
                }//Post


                await next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Something went error, please contact Computer User Help Desk." }));
                return;
            }
        }

    }
}
