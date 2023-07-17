using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebCommonHelper.Config;
using WebCommonHelper.Entities;
using WebCommonHelper.Entities.Enum;

namespace WebCommonHelper.Services.Authenticaiton
{
    public class Jwe : IJwt
    {
        private readonly JwtOptions jwtOptions;

        public Jwe(IOptions<JwtOptions> jwtOptions)
        {
            this.jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(User user, Token tokenType)
        {
            if (string.IsNullOrEmpty(jwtOptions.SecretKey) ||
                string.IsNullOrEmpty(jwtOptions.CertPath) ||
                string.IsNullOrEmpty(jwtOptions.CertPublicKey))
            {
                throw new OptionMissingException("SecretKey or CertPath Missing");
            }

            // generate token that is valid for 30 minutes
            var certPath = jwtOptions.CertPath;
            var key = jwtOptions.CertPublicKey;
            var tokenHandler = new JwtSecurityTokenHandler();
            var expriedMinutes = tokenType == Token.AccessToken
                ? jwtOptions.TokenValidityInMinutes
                : jwtOptions.RefreshTokenValidityInHours * 60;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.UserId.ToString())
                }),
                Audience = jwtOptions.Audience,
                Issuer = jwtOptions.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(expriedMinutes),
                EncryptingCredentials = new X509EncryptingCredentials(new X509Certificate2(certPath, key))
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GenerateToken(Application order, Token tokenType = Token.AccessToken)
        {
            if (string.IsNullOrEmpty(jwtOptions.SecretKey) ||
                string.IsNullOrEmpty(jwtOptions.CertPath) ||
                string.IsNullOrEmpty(jwtOptions.CertPublicKey))
            {
                throw new OptionMissingException("SecretKey or CertPath Missing");
            }

            // generate token that is valid for 30 minutes
            var certPath = jwtOptions.CertPath;
            var key = jwtOptions.CertPublicKey;
            var tokenHandler = new JwtSecurityTokenHandler();
            var expriedMinutes = tokenType == Token.AccessToken
                ? jwtOptions.TokenValidityInMinutes
                : jwtOptions.RefreshTokenValidityInHours * 60;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", order.appId.ToString())
                }),
                Audience = jwtOptions.Audience,
                Issuer = jwtOptions.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(expriedMinutes),
                EncryptingCredentials = new X509EncryptingCredentials(new X509Certificate2(certPath, key))
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateToken( Token tokenType) {
            return "";
        }

        public List<Claim>? ValidateToken(string token, Token tokenType)
        {
            if (string.IsNullOrEmpty(token) ||
                string.IsNullOrEmpty(jwtOptions.CertPath) ||
                string.IsNullOrEmpty(jwtOptions.CertPublicKey)) return null;

            var certPath = jwtOptions.CertPath;
            var key = jwtOptions.CertPublicKey;
            var tokenHandler = new JwtSecurityTokenHandler();
            var expriedMinutes = tokenType == Token.AccessToken
                ? jwtOptions.TokenValidityInMinutes
                : jwtOptions.RefreshTokenValidityInHours * 60;

            try
            {
                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateLifetime = true,//是否驗證失效時間
                        ClockSkew = TimeSpan.Zero, //時間偏移量（允許誤差時間） TimeSpan.FromSeconds(30),
                        ValidateIssuerSigningKey = true, //是否驗證SecurityKey
                        ValidAudience = jwtOptions.Audience,
                        ValidIssuer = jwtOptions.Issuer,
                        RequireSignedTokens = false,
                        TokenDecryptionKey = new X509SecurityKey(new X509Certificate2(certPath, key))
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
