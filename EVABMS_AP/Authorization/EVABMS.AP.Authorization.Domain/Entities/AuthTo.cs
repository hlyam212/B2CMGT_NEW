using OracleAttribute.Attributes;
using Newtonsoft.Json;

namespace EVABMS.AP.Authorization.Domain.Entities
{
    /// <summary>
    /// MGT_AUTHORIZATION_TO
    /// </summary>
    [TableName("MGT_AUTHORIZATION_TO")]
    public class AuthTo
    {
        /// <summary>
        /// MGAT_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }
        /// <summary>
        /// 生效起始日
        /// </summary>
        [ColumnName("EffectiveStart", onlyQuery: true)]
        public string effectivestart { get; private set; }
        /// <summary>
        /// 生效截止日
        /// </summary>
        [ColumnName("EffectiveEnd", onlyQuery: true)]
        public string effectiveend { get; private set; }
        /// <summary>
        /// 生效起始日
        /// </summary>
        [ColumnName("EFFECTIVE_START")]
        public DateTime effectivedtstart { get; private set; }
        /// <summary>
        /// 生效截止日
        /// </summary>
        [ColumnName("EFFECTIVE_END")]
        public DateTime effectivedtend { get; private set; }
        /// <summary>
        /// 用Effective Date算出是否生效
        /// </summary>
        [ColumnName("state", onlyQuery: true)]
        public bool state { get; private set; }
        /// <summary>
        /// 被授權的USER_ROLE
        /// </summary>
        [ColumnName("USER_ROLE")]
        public string userrole { get; private set; }
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
        /// 用FK_MGAU_ID關聯這一筆UserRole是哪一筆功能的授權
        /// </summary>
        [ColumnName("FK_MGAU_ID")]
        public long fk_mgau_id { get; private set; }

        public AuthTo() { }

        [JsonConstructor]
        public AuthTo(long Id,
                      string EffectiveStart,
                      string EffectiveEnd,
                      DateTime EffectiveDtStart,
                      DateTime EffectiveDtEnd,
                      bool State,
                      string UserRole,
                      DateTime LastUpdatedTimestamp,
                      string LastUpdatedUserId,
                      long FK_MGAU_ID)
        {
            this.id = Id;
            this.effectivestart = EffectiveStart;
            this.effectiveend = EffectiveEnd;
            this.effectivedtstart = EffectiveDtStart;
            this.effectivedtend = EffectiveDtEnd;
            this.state = State;
            this.userrole = UserRole;
            this.lastupdatedtimestamp = LastUpdatedTimestamp;
            this.lastupdateduserid = LastUpdatedUserId;
            this.fk_mgau_id = FK_MGAU_ID;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static AuthTo Create(long Id,
                                    string EffectiveStart,
                                    string EffectiveEnd,
                                    DateTime EffectiveDtStart,
                                    DateTime EffectiveDtEnd,
                                    bool State,
                                    string UserRole,
                                    DateTime LastUpdatedTimestamp,
                                    string LastUpdatedUserId,
                                    long FK_MGAU_ID)
        {
            AuthTo model = new AuthTo(Id, EffectiveStart, EffectiveEnd, EffectiveDtStart, EffectiveDtEnd, State, UserRole, LastUpdatedTimestamp, LastUpdatedUserId, FK_MGAU_ID);
            return model;
        }

        /// <summary>
        /// Create
        /// </summary>
        public AuthTo SetStatus(DateTime start, DateTime?end)
        {
            this.state = Extension.DateIntervalDuplicateCheck(this.effectivedtstart, this.effectivedtend, start, end);
            return this;
        }
    }
}
