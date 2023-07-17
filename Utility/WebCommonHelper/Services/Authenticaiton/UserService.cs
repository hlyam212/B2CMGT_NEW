using CommonHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;
using System.Security.Claims;
using WebCommonHelper.Config;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;
using WebCommonHelper.Models;

namespace WebCommonHelper.Services.Authenticaiton
{
    public class UserService : IUserService
    {
        private static readonly string _userKey = "User";
        private User? user
        {
            get
            {
                return httpContext.Session.GetObject<User>(_userKey);
            }
            set
            {
                httpContext.Session.SetObject(_userKey, value);
            }
        }
        private readonly JwtOptions options;
        private readonly IIdentity identity;
        private readonly IJwt jwt;
        private HttpContext httpContext { get; set; }

        public UserService(IOptions<JwtOptions> options, IIdentity identity, IJwt jwt, IHttpContextAccessor httpContextAccessor)
        {
            this.options = options.Value;
            this.identity = identity;
            this.jwt = jwt;
            httpContext = httpContextAccessor.HttpContext;
        }

        [SupportedOSPlatform("windows")]
        public User Login(AuthenticateRequest request)
        {
            if (string.IsNullOrEmpty(request.UserAccount) ||
                string.IsNullOrEmpty(request.Password))
            {
                throw new WebCommonHelperException("User Info missing");
            }

            user = identity.Authenticate(request);
            if (string.IsNullOrEmpty(options.Audience))
            {
                options.Audience = user.UserId;
            }

            return user;
        }

        public AuthenticateResponse Authenticate(User user)
        {
            if (!user.Exist() ||
                string.IsNullOrEmpty(options.SecretKey))
            {
                // return null if user not found
                return new AuthenticateResponse(user, null, null);
            }

            string accessToken = jwt.GenerateToken(user, Token.AccessToken);
            string refreshToken = jwt.GenerateToken(user, Token.RefreshToken);

            return new AuthenticateResponse(user, accessToken, refreshToken);
        }
        public User? Validate(string token, Token tokenType = Token.AccessToken)
        {
            try
            {
                List<Claim> claims = jwt.ValidateToken(token, tokenType);

                return (claims != null)
                    ? new User(
                        accountId: claims.Find(x => x.Type == "id").Value,
                        unit: claims.Find(x => x.Type == "unit").Value,
                        mail: claims.Find(x => x.Type == "info").Value)
                    : null;
            }
            catch
            {
                return null;
            }
        }
        public string RefreshToken(User user)
        {
            return jwt.GenerateToken(user);
        }

        public User? GetUserById(string userId)
        {
            if (user == null || user.UserId != userId)
            {
                return GetUser();
            }
            return user;
        }
        public User? GetUser()
        {
            if (user == null)
            {
                try
                {
                    return httpContext.Session.GetObject<User>("User");
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return user;
        }

        public void SetTokenCookie(IResponseCookies cookie, string refreshToken)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(options.RefreshTokenValidityInHours),
                SameSite = SameSiteMode.Strict,
                IsEssential =true,
                Secure = true,
            };

            cookie.Append("RefreshToken", refreshToken, cookieOptions);
        }

        public void Logout()
        {
            user = null;
        }
    }
}
