using EncryptionHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WebCommonHelper.Config;

namespace WebCommonHelper.Middlewares
{
    /// <summary>
    /// controller回傳加密
    /// </summary>
    public class EncryptMiddleware
    {
        private readonly RequestDelegate next;
        private readonly APISettings apiSettings;

        public EncryptMiddleware(RequestDelegate _next, IOptions<APISettings> _apiSettings)
        {
            next = _next;
            apiSettings = _apiSettings.Value;
        }

        /// <summary>
        /// 程式順序會影響資料
        /// 先關聯memorystream到Response.Body
        /// next到下一動作(go to controller),取得Response.Body
        /// controller return後,回到middleware
        /// 修改Response.Body
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // 取得原始值,應為空白
                var originBody = context.Response.Body;

                // 先關聯memorystream到Response.Body
                var mem = new MemoryStream();
                context.Response.Body = mem;

                // next到下一動作(go to controller),取得Response.Body
                await next(context);

                // controller return後,回到middleware
                mem.Position = 0;

                // 取回傳值
                var responseBody = new StreamReader(mem).ReadToEnd();

                // 加密
                responseBody = EncryptionService.AesEncrypt(responseBody, apiSettings.Api.key, apiSettings.Api.salt);

                // 重新寫入Response.Body回傳
                var memModify = new MemoryStream();
                var sw = new StreamWriter(memModify);
                sw.Write(responseBody);
                sw.Flush();
                memModify.Position = 0;

                await memModify.CopyToAsync(originBody);
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
