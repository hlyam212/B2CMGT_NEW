using OracleAttribute.Attributes;
using Newtonsoft.Json;

namespace EVABMS.AP.Authorization.Domain.Entities
{
    /// <summary>
    /// MGT_AUTHORIZATION_SETTING_HISTORY
    /// </summary>
    [TableName("MGT_AUTHORIZATION_SETTING_HISTORY")]
    public class AuthSettingHistory
    {
        /// <summary>
        /// MGUH_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }
        /// <summary>
        /// AAMS_SYS_CODE
        /// </summary>
        [ColumnName("AAMS_SYS_CODE")]
        public string aamssyscode { get; private set; }
        /// <summary>
        /// 功能/權限的名稱
        /// </summary>
        [ColumnName("FUNCTION_NAME")]
        public string name { get; private set; }
        /// <summary>
        /// 說明
        /// </summary>
        [ColumnName("DESCRIPTION")]
        public string description { get; private set; }
        /// <summary>
        /// 在權限中屬於第幾層
        /// </summary>
        [ColumnName("LEVELS")]
        public long levels { get; private set; }
        /// <summary>
        /// Add/Modify/Delete
        /// </summary>
        [ColumnName("ACTION")]
        public string action { get; private set; }
        /// <summary>
        /// 最後修改時間
        /// </summary>
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime lastupdatedtimestamp { get; private set; }
        /// <summary>
        /// 最後修改人員
        /// </summary>
        [ColumnName("LAST_UPDATED_USERID")]
        public string lastupdateduserid { get; private set; }
        /// <summary>
        /// 這一筆功能/權限的Parent
        /// </summary>
        [ColumnName("FK_MGAU_ID")]
        public long fk_mgau_id { get; private set; }
        /// <summary>
        /// DB的環境
        /// </summary>
        [ColumnName("ENV")]
        public string env { get; private set; }

        public AuthSettingHistory() { }

        [JsonConstructor]
        public AuthSettingHistory(long Id,
                                  string AAMSSysCode,
                                  string Name,
                                  string Description,
                                  long Levels,
                                  string Action,
                                  DateTime LastUpdatedTimestamp,
                                  string LastUpdatedUserId,
                                  long FK_MGAU_ID,
                                  string ENV)
        {
            this.id = Id;
            this.aamssyscode = AAMSSysCode;
            this.name = Name;
            this.description = Description;
            this.levels = Levels;
            this.action = Action;
            this.lastupdatedtimestamp = LastUpdatedTimestamp;
            this.lastupdateduserid = LastUpdatedUserId;
            this.fk_mgau_id = FK_MGAU_ID;
            this.env = ENV;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static AuthSettingHistory Create(long Id,
                                                string AAMSSysCode,
                                                string Name,
                                                string Description,
                                                long Levels,
                                                string Action,
                                                DateTime LastUpdatedTimestamp,
                                                string LastUpdatedUserId,
                                                long FK_MGAU_ID,
                                                string ENV)
        {
            AuthSettingHistory model = new AuthSettingHistory(Id, AAMSSysCode, Name, Description, Levels, Action, LastUpdatedTimestamp, LastUpdatedUserId, FK_MGAU_ID, ENV);
            return model;
        }
    }
}
