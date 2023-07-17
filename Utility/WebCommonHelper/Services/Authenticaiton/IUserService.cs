using Microsoft.AspNetCore.Http;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;
using WebCommonHelper.Models;

namespace WebCommonHelper.Services.Authenticaiton
{
    public interface IUserService
    {
        User Login(AuthenticateRequest request);
        AuthenticateResponse Authenticate(User user);
        User? Validate(string token, Token tokenType = Token.AccessToken);
        string RefreshToken(User user);
        void SetTokenCookie(IResponseCookies cookie, string refreshToken);
        User? GetUser();
        User? GetUserById(string userId);
        void Logout();
    }
}
