using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Survey.Domain.Entities
{
    [TableName("MGT_LANG_FORM")]
    public class LangForm : BaseEntities
    {
        /// <summary>
        /// FK_MFOF_ID_SEQ
        /// </summary>
        [ColumnName("FK_MFOF_ID", indexes: new int[] { 1 })]
        public long fk_mfof_id { get; private set; }

        /// <summary>
        /// 語系
        /// </summary>
        [ColumnName("LANG", indexes: new int[] { 1 })]
        public string lang { get; private set; }

        /// <summary>
        /// 標題
        /// </summary>
        [ColumnName("KEY")]
        public string key { get; private set; }

        /// <summary>
        /// 標題名稱
        /// </summary>
        [ColumnName("VALUE")]
        public string value { get; private set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public string last_updated_timestamp { get; private set; }

        /// <summary>
        /// 最後修改人員
        /// </summary>
        [ColumnName("LAST_UPDATED_USERID")]
        public string last_updated_userid { get; private set; }

        /// <summary>
        /// 設定搜尋條件
        /// </summary>
        public LangForm SetMFOFID(long id, string lang)
        {
            this.fk_mfof_id = id;
            this.lang = lang;
            return this;
        }

    }
}
