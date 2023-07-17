using Microsoft.AspNetCore.Http;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;
using WebCommonHelper.Models;

namespace WebCommonHelper.Services.Authenticaiton
{
    public interface IAppService
    {
        AuthenticateResponse Authenticate(Application order);
        Application? Validate(string token, Token tokenType = Token.AccessToken);
        string RefreshToken(Application order);
        void SetTokenCookie(IResponseCookies cookie, string refreshToken);
        Application? GetApp();
        Application? GetAppById(string orderId);
    }
}
