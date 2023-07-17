using System.Text;

namespace WebCommonHelper.Config
{
    public class JwtOptions
    {
        public string? Audience { get; set; }
        public string? Issuer { get; set; }
        public string? SecretKey { get; set; }
        /// <summary>
        /// refresh token time to live (in days), inactive tokens are
        /// automatically deleted from the database after this time
        /// </summary>
        public int RefreshTokenTTL { get; set; }
        public double TokenValidityInMinutes { get; set; } = 30;
        public double RefreshTokenValidityInHours { get; set; } = 4;
        public string? CertPath { get; set; }
        public string? CertPublicKey { get; set; }
    }
}
