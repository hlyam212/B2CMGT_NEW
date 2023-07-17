using CommonHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using WebCommonHelper.Config;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;
using WebCommonHelper.Services.Authenticaiton;

namespace WebCommonHelper.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;

        public JwtMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var refreshToken = context.Request.Cookies["RefreshToken"]?.ToString();

            if (accessToken != null && refreshToken != null)
            {
                User? userFromAccessToken = userService.Validate(accessToken);
                User? userFormRefreshToken = userService.Validate(refreshToken, Token.RefreshToken);

                if (userFromAccessToken != null && userFormRefreshToken != null &&
                    userFromAccessToken.UserId == userFormRefreshToken.UserId)
                {
                    // attach user to context on successful jwt validation (Access Token)
                    User? user = userService.GetUserById(userFromAccessToken.UserId);
                    if (user == null)
                    {
                        string? userString = context.Session.GetString("User");
                        if (userString != null)
                        {
                            user = userString.Deserialize<User>();
                        }
                    }

                    if (user == null && userFromAccessToken == null)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Please login again, the connection has timed out." }));
                        return;
                    }
                    context.Items["User"] = user ?? userFromAccessToken;
                }
                else if (userFromAccessToken == null && userFormRefreshToken != null)
                {
                    User? user = userService.GetUserById(userFromAccessToken?.UserId);
                    if (user == null)
                    {
                        string? userString = context.Session.GetString("User");
                        if (userString != null)
                        {
                            user = userString.Deserialize<User>();
                        }
                    }

                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    context.Response.ContentType = "application/json";

                    if (user == null)
                    {
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Please login again, the connection has timed out." }));
                        return;
                    }

                    string newAccessToken = userService.RefreshToken(user);
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
