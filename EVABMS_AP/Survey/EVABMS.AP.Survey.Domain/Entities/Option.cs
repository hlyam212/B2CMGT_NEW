using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Survey.Domain.Entities
{
    [TableName("MGT_FORM_OPTION")]
    public class Option : BaseEntities
    {
        /// <summary>
        /// MFOO_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }

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
        /// 參數設定代表問卷選項資料屬於哪一題
        /// </summary>
        [ColumnName("FK_FROM_MFOQ_ID", indexes: new int[] { 1 })]
        public long fk_form_mfoq_id { get; private set; }

        /// <summary>
        /// 參數設定代表問卷選項資料下一題
        /// </summary>
        [ColumnName("FK_TO_MFOQ_ID")]
        public long? fk_to_mfoq_id { get; private set; }

        /// <summary>
        /// 選項所代表的值
        /// </summary>
        [ColumnName("OPTION_VALUE")]
        public string? option_value { get; private set; }

        /// <summary>
        /// 選項名稱
        /// </summary>
        [ColumnName("Option", onlyQuery: true)]
        public string? option { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ColumnName("Value", onlyQuery: true)]
        public string? value { get; private set; }

        public Option()
        {

        }

        [JsonConstructor]
        public Option(long id, DateTime lastupdatedtimestamp, string lastupdateduserid, long fk_form_mfoq_id, long? fk_to_mfoq_id, string? option_value, string? option, string? value)
        {
            this.id = id;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
            this.lastupdateduserid = lastupdateduserid;
            this.fk_form_mfoq_id = fk_form_mfoq_id;
            this.fk_to_mfoq_id = fk_to_mfoq_id;
            this.option_value = option_value;
            this.option = option;
            this.value = value;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static Option Create(long id, DateTime lastupdatedtimestamp, string lastupdateduserid, long fk_form_mfoq_id, long? fk_to_mfoq_id, string? option_value, string? option, string? value)
        {
            Option model = new Option(id, lastupdatedtimestamp, lastupdateduserid, fk_form_mfoq_id, fk_to_mfoq_id, option_value, option, value);
            return model;
        }

        public Option SetFKFROMMFOQID(long id)
        {
            this.fk_form_mfoq_id = id;
            return this;
        }

        public Option SetOPTION(string option)
        {
            this.option = option;
            return this;
        }
    }
}
