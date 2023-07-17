using OracleAttribute.Attributes;
using System.Text.Json.Serialization;

namespace EVABMS.AP.Authorization.Domain.Entities
{
    /// <summary>
    /// MGT_USER_ROLES
    /// </summary>
    [TableName("MGT_USER_ROLES")]
    public class UserRole
    {
        /// <summary>
        /// USER_ROLE
        /// </summary>
        [ColumnName("USER_ROLE", isPrimaryKey: true)]
        public string Role { get; private set; }
        /// <summary>
        /// 說明
        /// </summary>
        [ColumnName("DESCRIPTION")]
        public string Description { get; private set; }
        /// <summary>
        /// 最後修改時間
        /// </summary>
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime LastUpdatedTimestamp { get; private set; }
        /// <summary>
        /// 最後修改人員
        /// </summary>
        [ColumnName("LAST_UPDATED_USERID")]
        public string? LastUpdatedUserId { get; private set; }
        /// <summary>
        /// DB的環境
        /// </summary>
        [ColumnName("ENV")]
        public string? ENV { get; private set; }
        /// <summary>
        /// 是否可以修改
        /// </summary>
        [ColumnName("editable", onlyQuery: true)]
        public bool? editable { get; private set; }

        public UserRole() { }

        [JsonConstructor]
        public UserRole(string Role,
                        string Description,
                        DateTime LastUpdatedTimestamp,
                        string? LastUpdatedUserId,
                        string? ENV)
        {
            this.Role = Role;
            this.Description = Description;
            this.LastUpdatedTimestamp = LastUpdatedTimestamp;
            this.LastUpdatedUserId = LastUpdatedUserId;
            this.ENV = ENV;
        }

        public static UserRole Create(string Role,
                                      string Description,
                                      DateTime LastUpdatedTimestamp,
                                      string? LastUpdatedUserId,
                                      string? ENV)
        {
            UserRole model = new UserRole
            {
                Role = Role,
                Description = Description,
                LastUpdatedTimestamp = LastUpdatedTimestamp,
                LastUpdatedUserId = LastUpdatedUserId,
                ENV = ENV
            };
            return model;
        }

        /// <summary>
        /// Create
        /// </summary>
        public UserRole SetEditable(bool ind)
        {
            this.editable = ind;
            return this;
        }
    }
}
