using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Survey.Domain.Entities
{
    [TableName("MGT_FORM_ANS_SECTION")]
    public class AnsSection : BaseEntities
    {
        /// <summary>
        /// MFAS_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }

        /// <summary>
        /// 第幾份問卷編號
        /// </summary>
        [ColumnName("FK_MFAF_SEQ")]
        public long fkmfafseq { get; private set; }

        /// <summary>
        /// 區塊編號
        /// </summary>
        [ColumnName("FK_MFOS_ID")]
        public long fkmfosid { get; private set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime lastupdatedtimestamp { get; private set; }

        public AnsSection()
        {

        }

        [JsonConstructor]
        public AnsSection(long id, long fkmfafseq, long fkmfosid, DateTime lastupdatedtimestamp)
        {
            this.id = id;
            this.fkmfafseq = fkmfafseq;
            this.fkmfosid = fkmfosid;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static AnsSection Create(long id, long fkmfafseq, long fkmfosid, DateTime lastupdatedtimestamp)
        {
            AnsSection model = new AnsSection(id, fkmfafseq, fkmfosid, lastupdatedtimestamp);
            return model;
        }

    }
}
