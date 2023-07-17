using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Survey.Domain.Entities
{
    [TableName("MGT_FORM_QUESTION")]
    public class Question : BaseEntities
    {
        /// <summary>
        /// MFOQ_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ColumnName("MUST_ID")]
        public string? must_id { get; private set; }

        /// <summary>
        /// 回答類型
        /// </summary>
        [ColumnName("QUESTION_TYPE")]
        public string question_type { get; private set; }

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
        /// 參數設定代表問卷題目資料屬於哪份問卷的區塊
        /// </summary>
        [ColumnName("FK_MFOS_ID", indexes: new int[] { 1 })]
        public long fk_mfos_id { get; private set; }

        /// <summary>
        /// 參數設定代表下一題
        /// </summary>
        [ColumnName("FK_TO_MFOQ_ID")]
        public long? fk_to_mfoq_id { get; private set; }

        /// <summary>
        /// 問卷題目編號
        /// </summary>
        [ColumnName("SEQ")]
        public long seq { get; private set; }

        /// <summary>
        /// 問題名稱
        /// </summary>
        [ColumnName("QUESTION", onlyQuery: true)]
        public string? question { get; private set; }

        /// <summary>
        /// 選項
        /// </summary>
        [ColumnName("OPTION", onlyQuery: true)]
        public List<Option>? option { get; private set; }

        public Question()
        {

        }

        [JsonConstructor]
        public Question(long id, string? must_id, string question_type, DateTime lastupdatedtimestamp, string lastupdateduserid, long fk_mfos_id, long? fk_to_mfoq_id, long seq, string? question, List<Option>? option)
        {
            this.id = id;
            this.must_id = must_id;
            this.question_type = question_type;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
            this.lastupdateduserid = lastupdateduserid;
            this.fk_mfos_id = fk_mfos_id;
            this.fk_to_mfoq_id = fk_to_mfoq_id;
            this.seq = seq;
            this.question = question;
            this.option = option;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static Question Create(long id, string? must_id, string question_type, DateTime lastupdatedtimestamp, string lastupdateduserid, long fk_mfos_id, long? fk_to_mfoq_id, long seq, string? question, List<Option>? option)
        {
            Question model = new Question(id, must_id, question_type, lastupdatedtimestamp, lastupdateduserid, fk_mfos_id, fk_to_mfoq_id, seq, question, option);
            return model;
        }

        public Question SetMFOSID(long id)
        {
            this.fk_mfos_id = id;
            return this;
        }

        public Question SetOPTION(List<Option> options)
        {
            this.option = options;
            return this;
        }

        public Question SetQUESTION(string? question)
        { 
            this.question = question; 
            return this;
        }
    }
}
