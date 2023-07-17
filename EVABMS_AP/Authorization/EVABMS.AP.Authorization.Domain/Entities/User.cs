using CommonHelper;
using OracleAttribute.Attributes;
using System.Text.Json.Serialization;

namespace EVABMS.AP.Authorization.Domain.Entities
{
    /// <summary>
    /// MGT_USERS
    /// </summary>
    [TableName("MGT_USERS")]
    public class User
    {
        /// <summary>
        /// AD
        /// </summary>
        [ColumnName("USERID", isPrimaryKey: true)]
        public string UserId { get; private set; }
        /// <summary>
        /// 部門
        /// </summary>
        [ColumnName("DEPARTMENT")]
        public string Department { get; private set; }
        /// <summary>
        /// 最後修改時間
        /// </summary>
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime LastUpdatedTimestamp { get; private set; }
        /// <summary>
        /// 最後修改人員
        /// </summary>
        [ColumnName("LAST_UPDATED_USERID")]
        public string LastUpdatedUserId { get; private set; }
        /// <summary>
        /// 人員的中文名
        /// </summary>
        [ColumnName("USER_CNAME")]
        public string UserCName { get; private set; }
        /// <summary>
        /// 人員的英文名
        /// </summary>
        [ColumnName("USER_ENAME")]
        public string UserEName { get; private set; }
        /// <summary>
        /// /// <summary>
        /// 人員的Email
        /// </summary>
        [ColumnName("EMAIL")]
        public string Email { get; private set; }
        /// <summary>
        /// AAMS_SYS_CODE
        /// </summary>
        [ColumnName("AAMS_SYS_CODE")]
        public string AAMSSYSCode { get; private set; }
        /// <summary>
        /// AAMS_CATEGORY
        /// </summary>
        [ColumnName("AAMS_CATEGORY")]
        public string AAMSCategory { get; private set; }
        /// <summary>
        /// 最後登入的時間戳記
        /// </summary>
        [ColumnName("LAST_LOGIN_TIMESTAMP")]
        public DateTime? LastLoginTimestamp { get; private set; }
        /// <summary>
        /// DB的環境
        /// </summary>
        [ColumnName("ENV")]
        public string? ENV { get; private set; }

        public User() { }

        [JsonConstructor]
        public User(string Department,
                    string UserId,
                    DateTime LastUpdatedTimestamp,
                    string LastUpdatedUserId,
                    string UserCName,
                    string UserEName,
                    string Email,
                    string AAMSSYSCode,
                    string AAMSCategory,
                    DateTime? LastLoginTimestamp)
        {
            this.Department = Department;
            this.UserId = UserId;
            this.LastUpdatedTimestamp = LastUpdatedTimestamp;
            this.LastUpdatedUserId = LastUpdatedUserId;
            this.UserCName = UserCName;
            this.UserEName = UserEName;
            this.Email = Email;
            this.AAMSSYSCode = AAMSSYSCode;
            this.AAMSCategory = AAMSCategory;
            this.LastLoginTimestamp = LastLoginTimestamp;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static User Create(string Department,
                                  string UserId,
                                  DateTime LastUpdatedTimestamp,
                                  string LastUpdatedUserId,
                                  string UserCName,
                                  string UserEName,
                                  string Email,
                                  string AAMSSYSCode,
                                  string AAMSCategory,
                                  DateTime? LastLoginTimestamp)
        {
            User model = new User(Department, UserId, LastUpdatedTimestamp, LastUpdatedUserId, UserCName, UserEName, Email, AAMSSYSCode, AAMSCategory, LastLoginTimestamp);
            return model;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static User Clone(User duplicate) 
        {
            User model = duplicate.DeepCopy();
            return model;
        }

        /// <summary>
        /// SetAD
        /// </summary>
        public User SetAD(string AD)
        {
            this.UserId = AD;
            return this;
        }

        /// <summary>
        /// SetAD
        /// </summary>
        public User SetLoginTimestamp()
        {
            this.LastLoginTimestamp = DateTime.Now;
            return this;
        }
    }
}
