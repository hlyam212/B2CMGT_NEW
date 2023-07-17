using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Survey.Domain.Entities
{
    [TableName("MGT_FORM_SECTION")]
    public class Section : BaseEntities
    {
        /// <summary>
        /// MFOS_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }

        /// <summary>
        /// 區塊
        /// </summary>
        [ColumnName("SEQ")]
        public long seq { get; private set; }

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
        /// 參數設定代表區塊資料屬於哪份問卷
        /// </summary>
        [ColumnName("FK_MFOF_ID", indexes: new int[] { 1 })]
        public long fk_mfof_id { get; private set; }

        /// <summary>
        /// 區塊名稱
        /// </summary>
        [ColumnName("TITLE", onlyQuery: true)]
        public string? title { get; private set; }

        /// <summary>
        /// 問題名稱
        /// </summary>
        [ColumnName("QUESTION", onlyQuery: true)]
        public List<Question> question { get; private set; }

        public Section()
        {

        }

        [JsonConstructor]
        public Section(long id, long seq, DateTime lastupdatedtimestamp, string lastupdateduserid, long fk_mfof_id, string? title, List<Question> question)
        {
            this.id = id;
            this.seq = seq;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
            this.lastupdateduserid = lastupdateduserid;
            this.fk_mfof_id = fk_mfof_id;
            this.title = title;
            this.question = question;
        }

        public Section SetMFOFID(long id)
        {
            this.fk_mfof_id = id;
            return this;
        }

        public Section SetQUESTION(List<Question> question)
        {
            this.question = question;
            return this;
        }

        public Section SetTITLE(string? title)
        { 
            this.title = title; 
            return this;
        }
    }
}
