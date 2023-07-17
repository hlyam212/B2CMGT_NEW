using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Survey.Domain.Entities
{
    [TableName("MGT_FORM_ANS_OPTION")]
    public class AnsOption : BaseEntities
    {
        /// <summary>
        /// MFAO_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }

        /// <summary>
        /// FK_MFAQ_SEQ
        /// </summary>
        [ColumnName("FK_MFAQ_SEQ")]
        public long fkmfaqseq { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ColumnName("FK_MFOO_ID")]
        public long fkmfooid { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ColumnName("Value")]
        public string? value { get; private set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime lastupdatedtimestamp { get; private set; }

        public AnsOption()
        {

        }

        [JsonConstructor]
        public AnsOption(long id, long fkmfaqseq, long fkmfooid, string? value, DateTime lastupdatedtimestamp)
        {
            this.id = id;
            this.fkmfaqseq = fkmfaqseq;
            this.fkmfooid = fkmfooid;
            this.value = value;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static AnsOption Create(long id, long fkmfaqseq, long fkmfooid, string? value, DateTime lastupdatedtimestamp)
        {
            AnsOption model = new AnsOption(id, fkmfaqseq, fkmfooid, value, lastupdatedtimestamp);
            return model;
        }

    }
}
