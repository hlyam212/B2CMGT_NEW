using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;


namespace EVABMS.AP.Survey.Domain.Entities
{
    [TableName("MGT_FORM_ANS")]
    public class AnsForm : BaseEntities
    {
        /// <summary>
        /// MFAF_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }

        /// <summary>
        /// 語系
        /// </summary>
        [ColumnName("LANG")]
        public string lang { get; set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime lastupdatedtimestamp { get; set; }
        /// <summary>
        /// fk_mfof_id
        /// </summary>
        [ColumnName("FK_MFOF_ID")]
        public long fkmfofid { get; set; }

        public AnsForm()
        {

        }

        [JsonConstructor]
        public AnsForm(long id, string lang, DateTime lastupdatedtimestamp, long fkmfofid)
        {
            this.id = id;
            this.lang = lang;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
            this.fkmfofid = fkmfofid;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static AnsForm Create(long id, string lang, DateTime lastupdatedtimestamp, long fkmfofid)
        {
            AnsForm model = new AnsForm(id, lang, lastupdatedtimestamp, fkmfofid);
            return model;
        }

        /// <summary>
        /// 設定搜尋條件
        /// </summary>
        public AnsForm SetQuery(long id)
        {
            this.id = id;
            return this;
        }

        /// <summary>
        /// MFAF_SEQ
        /// </summary>
        public AnsForm SetID(long id)
        {
            this.id = id;
            return this;
        }

    }
}