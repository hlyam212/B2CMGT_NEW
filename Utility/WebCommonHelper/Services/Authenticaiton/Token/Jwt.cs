using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebCommonHelper.Config;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;

namespace WebCommonHelper.Services.Authenticaiton
{
    public class Jwt : IJwt
    {
        private readonly JwtOptions jwtOptions;
        public Jwt(IOptions<JwtOptions> jwtOptions)
        {
            this.jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(User user, Token tokenType = Token.AccessToken)
        {
            if (string.IsNullOrEmpty(jwtOptions.SecretKey))
            {
                throw new OptionMissingException("SecretKey Missing");
            }

            // generate token that is valid for 30 minutes
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes($@"{jwtOptions.SecretKey}_{tokenType.ToString()}");
            var expriedMinutes = tokenType == Token.AccessToken
                ? jwtOptions.TokenValidityInMinutes
                : jwtOptions.RefreshTokenValidityInHours * 60;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.UserId.ToString()),
                    new Claim("unit", string.IsNullOrEmpty(user.OfficeCode)?"":user.OfficeCode.ToString()),
                    new Claim("info", string.IsNullOrEmpty(user.Mail)?"":user.Mail.ToString())
                }),
                Audience = user.UserId.ToString(),
                Issuer = jwtOptions.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(expriedMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);


        }
        public string GenerateToken(Application order, Token tokenType = Token.AccessToken)
        {
            if (string.IsNullOrEmpty(jwtOptions.SecretKey))
            {
                throw new OptionMissingException("SecretKey Missing");
            }

            // generate token that is valid for 30 minutes
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes($@"{jwtOptions.SecretKey}_{tokenType.ToString()}");
            var expriedMinutes = tokenType == Token.AccessToken
                ? jwtOptions.TokenValidityInMinutes
                : jwtOptions.RefreshTokenValidityInHours * 60;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", order.appId.ToString()),
                }),
                Audience = order.appId.ToString(),
                Issuer = jwtOptions.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(expriedMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

        public string GenerateToken(Token tokenType = Token.AccessToken)
        {
            if (string.IsNullOrEmpty(jwtOptions.SecretKey))
            {
                throw new OptionMissingException("SecretKey Missing");
            }

            // generate token that is valid for 30 minutes
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes($@"{jwtOptions.SecretKey}_{tokenType.ToString()}");
            var expriedMinutes = tokenType == Token.AccessToken
                ? jwtOptions.TokenValidityInMinutes
                : jwtOptions.RefreshTokenValidityInHours * 60;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", jwtOptions.Issuer)
                }),
                Audience = jwtOptions.Issuer.ToString(),
                Issuer = jwtOptions.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(expriedMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        public List<Claim>? ValidateToken(string token, Token tokenType = Token.AccessToken)
        {
            if (string.IsNullOrEmpty(token)) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes($@"{jwtOptions.SecretKey}_{tokenType.ToString()}");

            try
            {
                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateLifetime = true,//是否驗證失效時間
                        ClockSkew = TimeSpan.Zero, //時間偏移量（允許誤差時間） TimeSpan.FromSeconds(30),
                        ValidateIssuerSigningKey = true, //是否驗證SecurityKey
                        IssuerSigningKey = new SymmetricSecurityKey(key), //拿到祕鑰SecurityKey
                        ValidateIssuer = false, //是否驗證Issuer（頒發者）
                        ValidateAudience = false, //是否驗證Audience（驗證之前的token是否失效）

                    },
                    out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // 是否已過期
                if (DateTime.UtcNow > jwtToken.ValidTo)
                {
                    return null;
                }

                return jwtToken.Claims.ToList();
            }
            catch
            {
                // return null if validation fails
                return null;
            }

        }
    }
}
