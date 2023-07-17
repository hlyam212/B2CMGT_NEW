using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;


namespace EVABMS.AP.Survey.Domain.Entities
{
    [TableName("MGT_FORM")]
    public class Form : BaseEntities
    {
        /// <summary>
        /// MFOF_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }

        /// <summary>
        /// 語系
        /// </summary>
        [ColumnName("DEFAULT_LANG")]
        public string default_lang { get; set; }

        /// <summary>
        /// 生效日
        /// </summary>
        [ColumnName("EFFECTIVE_START")]
        public DateTime? effective_start { get; private set; }

        /// <summary>
        /// 失效日
        /// </summary>
        [ColumnName("EFFECTIVE_END")]
        public DateTime? effective_end { get; private set; }

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
        /// DB的環境
        /// </summary>
        [ColumnName("ENV")]
        public string env { get; private set; }

        /// <summary>
        /// 標題
        /// </summary>
        [ColumnName("SUBJECT", onlyQuery:true)]
        public string? subject { get; private set; }

        /// <summary>
        /// 標題名稱
        /// </summary>
        [ColumnName("DESCRIPTION", onlyQuery:true)]
        public string? description { get; private set; }

        public Form()
        {

        }

        [JsonConstructor]
        public Form(long id, string default_lang, DateTime? effective_start, DateTime? effective_end, DateTime lastupdatedtimestamp, string lastupdateduserid, string env, string? subject, string? description)
        {
            this.id = id;
            this.default_lang = default_lang;
            this.effective_start = effective_start;
            this.effective_end = effective_end;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
            this.lastupdateduserid = lastupdateduserid;
            this.env = env;
            this.subject = subject;
            this.description = description;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static Form Create(long id, string default_lang, DateTime? effective_start, DateTime? effective_end, DateTime lastupdatedtimestamp, string lastupdateduserid, string env, string? subject, string? description)
        {
            Form model = new Form(id, default_lang, effective_start, effective_end, lastupdatedtimestamp, lastupdateduserid, env, subject, description);
            return model;
        }

        /// <summary>
        /// 設定搜尋條件
        /// </summary>
        public Form SetQuery(long id)
        {
            this.id = id;
            return this;
        }
    }
}