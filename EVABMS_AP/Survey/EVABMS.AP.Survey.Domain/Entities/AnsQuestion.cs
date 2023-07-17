using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Survey.Domain.Entities
{
    [TableName("MGT_FORM_ANS_QUESTION")]
    public class AnsQuestion : BaseEntities
    {
        /// <summary>
        /// MFAQ_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ColumnName("FK_MFAS_SEQ")]
        public long fkmfasseq { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ColumnName("FK_MFOQ_ID")]
        public long fkmfoqid { get; private set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime lastupdatedtimestamp { get; private set; }

        public AnsQuestion()
        {

        }

        [JsonConstructor]
        public AnsQuestion(long id, long fkmfasseq, long fkmfoqid, DateTime lastupdatedtimestamp)
        {
            this.id = id;
            this.fkmfasseq = fkmfasseq;
            this.fkmfoqid = fkmfoqid;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static AnsQuestion Create(long id, long fkmfasseq, long fkmfoqid, DateTime lastupdatedtimestamp)
        {
            AnsQuestion model = new AnsQuestion(id, fkmfasseq, fkmfoqid, lastupdatedtimestamp);
            return model;
        }

    }
}
