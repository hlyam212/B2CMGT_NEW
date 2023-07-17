using CommonHelper;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;
using WebCommonHelper.Services.Authenticaiton;

namespace WebCommonHelper.Middlewares
{
    public class OrderJwtMiddleware
    {
        private readonly RequestDelegate next;

        public OrderJwtMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IAppService orderService)
        {
            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var refreshToken = context.Request.Cookies["RefreshToken"]?.ToString();

            if (accessToken != null && refreshToken != null)
            {
                Application? orderFromAccessToken = orderService.Validate(accessToken);
                Application? orderFromRefreshToken = orderService.Validate(refreshToken, Token.RefreshToken);

                if (orderFromAccessToken != null &&
                    orderFromRefreshToken != null &&
                    orderFromAccessToken.appNo == orderFromRefreshToken.appNo)
                {
                    // attach user to context on successful jwt validation (Access Token)
                    Application? order = orderService.GetAppById(orderFromAccessToken.appNo);
                    if (order == null)
                    {
                        string? orderString = context.Session.GetString("Order");
                        if (orderString != null)
                        {
                            order = orderString.Deserialize<Application>();
                        }
                    }

                    // 只要Session 或 Token其中一個活著就好
                    if (order == null && orderFromAccessToken == null)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = $@"Please try again, the connection has timed out..." }));
                        return;
                    }

                    context.Items["Order"] = order ?? orderFromAccessToken;
                }
                else if (orderFromAccessToken == null && orderFromRefreshToken != null)
                {
                    // attach user to context on successful jwt validation (Access Token)
                    Application? order = orderService.GetAppById(orderFromAccessToken.appNo);
                    if (order == null)
                    {
                        string? orderString = context.Session.GetString("Order");
                        if (orderString != null)
                        {
                            order = orderString.Deserialize<Application>();
                        }
                    }

                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    context.Response.ContentType = "application/json";

                    if (order == null)
                    {
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Please try again, the connection has timed out.." }));
                        return;
                    }

                    string newAccessToken = orderService.RefreshToken(order);
                    await context.Response.WriteAsync(JsonSerializer.Serialize(
                        new
                        {
                            accessToken = newAccessToken
                        }));
                    return;
                }
            }

            await next(context);
        }
    }
}
