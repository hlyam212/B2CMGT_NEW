﻿using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Survey.Domain.Entities
{
    [TableName("MGT_LANG_FORM_OPTION")]
    public class LangOption : BaseEntities
    {
        /// <summary>
        /// FK_MFOO_ID_SEQ
        /// </summary>
        [ColumnName("FK_MFOO_ID")]
        public long fk_mfoo_id { get; private set; }

        /// <summary>
        /// 語系
        /// </summary>
        [ColumnName("LANG")]
        public string lang { get; private set; }

        /// <summary>
        /// 選項
        /// </summary>
        [ColumnName("KEY")]
        public string key { get; private set; }

        /// <summary>
        /// 選項答案
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
    }
}
