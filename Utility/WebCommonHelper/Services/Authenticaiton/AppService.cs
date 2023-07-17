using CommonHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebCommonHelper.Config;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;
using WebCommonHelper.Models;

namespace WebCommonHelper.Services.Authenticaiton
{
    public class AppService : IAppService
    {
        private Application? application { get; set; }
        private readonly JwtOptions options;
        private readonly IJwt jwt;
        private HttpContext httpContext { get; set; }

        public AppService(IOptions<JwtOptions> options, IJwt jwt, IHttpContextAccessor httpContextAccessor)
        {
            this.options = options.Value;
            this.jwt = jwt;
            httpContext = httpContextAccessor.HttpContext;
        }

        public AuthenticateResponse Authenticate(Application app)
        {
            if (!app.Exist() ||
                string.IsNullOrEmpty(options.SecretKey))
            {
                // return null if user not found
                return new AuthenticateResponse(app, null, null);
            }

            string accessToken = jwt.GenerateToken(app, Token.AccessToken);
            string refreshToken = jwt.GenerateToken(app, Token.RefreshToken);

            return new AuthenticateResponse(app, accessToken, refreshToken);
        }

        public Application? Validate(string token, Token tokenType = Token.AccessToken)
        {
            List<Claim> claims = jwt.ValidateToken(token, tokenType);

            try
            {
                return (claims != null)
                    ? new Application(
                        appId: claims.Find(x => x.Type == "id").Value,
                        appNo: claims.Find(x => x.Type == "no").Value,
                        appInfo: claims.Find(x => x.Type == "info").Value)
                    : null;
            }
            catch
            {
                return null;
            }
        }

        public Application? GetAppById(string appId)
        {
            if (application == null || application.appId != appId)
            {
                return GetApp();
            }
            return application;
        }
        public Application? GetApp()
        {
            if (application == null)
            {
                try
                {
                    return httpContext.Session.GetObject<Application>("App");
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return application;
        }

        public string RefreshToken(Application app)
        {
            return jwt.GenerateToken(app);
        }

        public void SetTokenCookie(IResponseCookies cookie, string refreshToken)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(options.RefreshTokenValidityInHours),
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
                Secure = true,
            };

            cookie.Append("RefreshToken", refreshToken, cookieOptions);
        }
    }
}
