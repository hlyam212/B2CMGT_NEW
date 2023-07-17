using Newtonsoft.Json;
using OracleAttribute.Attributes;
using UtilityHelper;

namespace EVABMS.AP.ConnectingString.Domain.Entities
{
    public class ConnectingStringQuery : BaseEntities
    {
        public long No { get; private set; }
        public string? Name { get; private set; }
        public string? DataSource { get; private set; }
        public string? UserId { get; private set; }
        public string? Password { get; private set; }
        public string? Other { get; private set; }
        public string? UserRole { get; private set; }

        public ConnectingStringQuery() { }

        [JsonConstructor]
        public ConnectingStringQuery(long No, string? Name, string? DataSource, string? UserId, string? Password, string? Other, string? UserRole)
        {
            this.No = No;
            this.Name = Name;
            this.DataSource = DataSource;
            this.UserId = UserId;
            this.Password = Password;
            this.Other = Other;
            this.UserRole = UserRole;
        }

        public static ConnectingStringQuery Create(long No, string? Name, string? DataSource, string? UserId, string? Password, string? Other, string? UserRole)
        {
            ConnectingStringQuery moedl = new ConnectingStringQuery(No, Name, DataSource, UserId, Password, Other, UserRole);
            return moedl;
        }

        public ConnectingStringQuery SetValue(string type, string val)
        {
            switch (type)
            {
                case "DATA SOURCE":
                    this.DataSource = val;
                    return this;
                case "PASSWORD":
                    this.Password = val;
                    return this;
                case "USER ID":
                    this.UserId = val;
                    return this;
                case "USER ROLE":
                    this.UserRole = val;
                    return this;
                default:
                    this.Other = val;
                    return this;
            }
        }
    }
}
