using System.Text.Json.Serialization;
using WebCommonHelper.Entities.Enum;

namespace WebCommonHelper.Entities
{
    public class User
    {
        public Company Company { get; init; }
        public string UserId { get; init; }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? OfficeCode { get; set; }
        public string? JobTitle { get; set; }
        public string? Mail { get; set; }
        public List<Role>? Roles { get; set; }

        public User() { }

        public User(Company company, string accountId)
        {
            Company = company;
            UserId = accountId;
        }

        public User(string accountId, string unit, string mail)
        {
            UserId = accountId;
            OfficeCode = unit;
            Mail = mail;
        }

        public bool Exist()
        {
            return !string.IsNullOrEmpty(FullName) &&
                !string.IsNullOrEmpty(OfficeCode);
        }
    }
}
