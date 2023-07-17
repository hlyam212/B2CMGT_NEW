using System.Security.Claims;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;

namespace WebCommonHelper.Services.Authenticaiton
{
    public interface IJwt
    {
        public string GenerateToken(User user, Token tokenType = Token.AccessToken);
        public string GenerateToken(Application order, Token tokenType = Token.AccessToken);

        public string GenerateToken(Token tokenType = Token.AccessToken);
        public List<Claim>? ValidateToken(string token, Token tokenType = Token.AccessToken);
    }
}
