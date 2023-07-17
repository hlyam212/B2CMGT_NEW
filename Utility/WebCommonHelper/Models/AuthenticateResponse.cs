using System.Text.Json.Serialization;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;

namespace WebCommonHelper.Models
{
    public class AuthenticateResponse
    {
        public string UserId { get; init; }
        public string FullName { get; init; }
        public string OfficeCode { get; init; }
        public List<Role> Roles { get; private set; }
        public string AccessToken { get; init; }
        public string RedirectUrl { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; private set; }

        public AuthenticateResponse(User user, string? accessToken, string? refreshToken)
        {
            if (string.IsNullOrEmpty(user.FullName) ||
                string.IsNullOrEmpty(user.OfficeCode) ||
                string.IsNullOrEmpty(accessToken) ||
                string.IsNullOrEmpty(refreshToken))
            {
                throw new Exception("Authentication Failed.");
            }

            UserId = user.UserId;
            FullName = user.FullName;
            OfficeCode = user.OfficeCode;
            Roles = new List<Role>();
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public AuthenticateResponse(Application order, string? accessToken, string? refreshToken)
        {
            if (string.IsNullOrEmpty(order.appNo) ||
                string.IsNullOrEmpty(order.appInfo) ||
                string.IsNullOrEmpty(accessToken) ||
                string.IsNullOrEmpty(refreshToken))
            {
                throw new Exception("Authentication Failed.");
            }

            UserId = order.appId;
            FullName = order.appNo;
            OfficeCode = order.appInfo;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public void SetRoles(List<Role> roles)
        {
            Roles = roles;
        }
    }
}
