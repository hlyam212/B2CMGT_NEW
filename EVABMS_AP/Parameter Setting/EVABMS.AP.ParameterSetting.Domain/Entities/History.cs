using Newtonsoft.Json;
using OracleAttribute.Attributes;
using UtilityHelper;

namespace EVABMS.AP.Parameter.Domain.Entities
{
    /// <summary>
    /// Parameter Setting的異動紀錄
    /// </summary>
    [TableName("MGT_PARAMETER_SETTINGS_HISTORY")]
    public class History : BaseEntities
    {
        /// <summary>
        /// MGPH_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; set; }
        /// <summary>
        /// 功能名稱
        /// </summary>
        [ColumnName("FUNCTION_NAME", indexes: new int[] { 1 })]
        public string? functionname { get; set; }
        /// <summary>
        /// 子功能名稱
        /// </summary>
        [ColumnName("SUB_FUNCTION_NAME", indexes: new int[] { 1 })]
        public string? subfunctionname { get; set; }
        /// <summary>
        /// 參數名稱：functionname/subfunctionname/settingname 為一組Key值
        /// functionname+subfunctionname+settingname=>可以找到唯一一組參數
        /// </summary>
        [ColumnName("SETTING_NAME", indexes: new int[] { 1 })]
        public string? settingname { get; set; }
        /// <summary>
        /// 參數值
        /// </summary>
        [ColumnName("VALUE")]
        public string? value { get; set; }
        /// <summary>
        /// Add/Modify/Delete
        /// </summary>
        [ColumnName("ACTION")]
        public string? action { get; set; }
        /// <summary>
        /// 最後修改時間
        /// </summary>
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime lastupdatedtimestamp { get; set; }
        /// <summary>
        /// 最後修改人員
        /// </summary>
        [ColumnName("LAST_UPDATED_USERID")]
        public string? lastupdateduserid { get; set; }
        /// <summary>
        /// 參數設定的授權
        /// </summary>
        [ColumnName("AUTHORIZED_TO")]
        public string? authorizeto { get; set; }
        /// <summary>
        /// DB的環境
        /// </summary>
        [ColumnName("ENV")]
        public string? ENV { get; private set; }

        public History() 
        {

        }

        [JsonConstructor]
        public History(long id, string? functionname, string? subfunctionname, string? settingname, string? value, string? action, DateTime lastupdatedtimestamp, string? lastupdateduserid, string authorizeto, string ENV)
        {
            this.id = id;
            this.functionname = functionname;
            this.subfunctionname = subfunctionname;
            this.settingname = settingname;
            this.value = value;
            this.action = action;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
            this.lastupdateduserid = lastupdateduserid;
            this.authorizeto = authorizeto;
            this.ENV = ENV;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static History Create(long id, string? functionname, string? subfunctionname, string? settingname, string? value, string? action, DateTime lastupdatedtimestamp, string? lastupdateduserid, string authorizeto, string ENV)
        {
            History model = new History
            {
                id = id,
                functionname = functionname,
                subfunctionname = subfunctionname,
                settingname = settingname,
                value = value,
                action = action,
                lastupdatedtimestamp = lastupdatedtimestamp,
                lastupdateduserid = lastupdateduserid,
                authorizeto = authorizeto,
                ENV = ENV
            };
            return model;
        }

        /// <summary>
        /// 設定尋條件
        /// </summary>
        public History SetQuery(string functionname, string subfunctionname, string settingname)
        {
            this.functionname = functionname;
            this.subfunctionname = subfunctionname;
            this.settingname = settingname;
            return this;
        }

        /// <summary>
        /// MGPH_SEQ
        /// </summary>
        public History SetID(long id)
        {
            this.id = id;
            return this;
        }

        /// <summary>
        /// SetLastUpdatedTimestamp
        /// </summary>
        public History SetLastUpdatedTimestamp()
        {
            this.lastupdatedtimestamp = DateTime.Now;
            return this;
        }
    }
}
