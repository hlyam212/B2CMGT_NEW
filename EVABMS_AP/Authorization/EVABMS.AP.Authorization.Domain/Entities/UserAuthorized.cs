using OracleAttribute.Attributes;
using System.Text.Json.Serialization;

namespace EVABMS.AP.Authorization.Domain.Entities
{
    /// <summary>
    /// MGT_AUTHORIZED_USERS
    /// </summary>
    [TableName("MGT_AUTHORIZED_USERS")]
    public class UserAuthorized
    {
        /// <summary>
        /// MGUU_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long Id { get; private set; }
        /// <summary>
        /// MGT_USERS.ID
        /// </summary>
        [ColumnName("FK_MGUS_SEQ")]
        public long? FK_MGUS_SEQ { get; private set; }
        /// <summary>
        /// USER_ROLE
        /// </summary>
        [ColumnName("USER_ROLE")]
        public string? UserRole { get; private set; }
        /// <summary>
        /// EFFECTIVE_START
        /// </summary>
        [ColumnName("EFFECTIVE_START")]
        public DateTime? EffectiveStart { get; private set; }
        /// <summary>
        /// EFFECTIVE_END
        /// </summary>
        [ColumnName("EFFECTIVE_END")]
        public DateTime? EffectiveEnd { get; private set; }
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

        public UserAuthorized() { }

        [JsonConstructor]
        public UserAuthorized(long Id,
                              long? FK_MGUS_SEQ,
                              string? UserRole,
                              DateTime? EffectiveStart,
                              DateTime? EffectiveEnd,
                              DateTime LastUpdatedTimestamp,
                              string LastUpdatedUserId)
        {
            this.Id = Id;
            this.FK_MGUS_SEQ = FK_MGUS_SEQ;
            this.UserRole = UserRole;
            this.EffectiveStart = EffectiveStart;
            this.EffectiveEnd = EffectiveEnd;
            this.LastUpdatedTimestamp = LastUpdatedTimestamp;
            this.LastUpdatedUserId = LastUpdatedUserId;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static UserAuthorized Create(long Id,
                                            long? FK_MGUS_SEQ,
                                            string? UserRole,
                                            DateTime? EffectiveStart,
                                            DateTime? EffectiveEnd,
                                            DateTime LastUpdatedTimestamp,
                                            string LastUpdatedUserId)
        {
            UserAuthorized model = new UserAuthorized(Id, FK_MGUS_SEQ, UserRole, EffectiveStart, EffectiveEnd, LastUpdatedTimestamp, LastUpdatedUserId);
            return model;
        }
    }
}
