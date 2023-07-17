using System.ComponentModel.DataAnnotations;
using WebCommonHelper.Entities.Enum;

namespace WebCommonHelper.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public Company UserCompany { get; set; } = Company.EVA;

        [Required]
        public string UserAccount { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
