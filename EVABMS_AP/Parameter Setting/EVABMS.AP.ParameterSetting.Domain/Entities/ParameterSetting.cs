using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Parameter.Domain.Entities
{
    /// <summary>
    /// Parameter Setting功能
    /// </summary>
    [TableName("MGT_PARAMETER_SETTINGS")]
    public class ParameterSetting : BaseEntities
    {
        /// <summary>
        /// MGPS_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }
        /// <summary>
        /// 功能名稱
        /// </summary>
        [ColumnName("FUNCTION_NAME", indexes: new int[] { 1,2 })]
        public string? functionname { get; private set; }
        /// <summary>
        /// 子功能名稱
        /// </summary>
        [ColumnName("SUB_FUNCTION_NAME", indexes: new int[] { 1,2 })]
        public string? subfunctionname { get; private set; }
        /// <summary>
        /// 參數名稱：functionname/subfunctionname/settingname 為一組Key值
        /// functionname+subfunctionname+settingname=>可以找到唯一一組參數
        /// </summary>
        [ColumnName("SETTING_NAME", indexes: new int[] { 1 })]
        public string? settingname { get; private set; }
        /// <summary>
        /// 參數值
        /// </summary>
        [ColumnName("VALUE")]
        public string? value { get; private set; }
        /// <summary>
        /// 說明
        /// </summary>
        [ColumnName("DESCRIPTION")]
        public string? description { get; private set; }
        /// <summary>
        /// 最後修改時間
        /// </summary>
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime lastupdatedtimestamp { get; private set; }
        /// <summary>
        /// 最後修改人員
        /// </summary>
        [ColumnName("LAST_UPDATED_USERID")]
        public string? lastupdateduserid { get; private set; }
        /// <summary>
        /// 參數設定授權給擁有指定權限的人員修改
        /// </summary>
        [ColumnName("FK_MGAU_ID")]
        public long fk_mgau_id { get; private set; }
        /// <summary>
        /// 指定權限的Name
        /// </summary>
        [ColumnName(name: "Auth", onlyQuery: true)]
        public string? Auth { get; private set; }
        /// <summary>
        /// DB的環境
        /// </summary>
        [ColumnName("ENV")]
        public string? ENV { get; private set; }

        public ParameterSetting()
        {

        }

        [JsonConstructor]
        public ParameterSetting(long id, string? functionname, string? subfunctionname, string? settingname, string? value, string? description, DateTime lastupdatedtimestamp, string? lastupdateduserid, long fk_mgau_id, string ENV)
        {
            this.id = id;
            this.functionname = functionname;
            this.subfunctionname = subfunctionname;
            this.settingname = settingname;
            this.value = value;
            this.description = description;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
            this.lastupdateduserid = lastupdateduserid;
            this.fk_mgau_id = fk_mgau_id;
            this.ENV = ENV;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static ParameterSetting Create(long id, string? functionname, string? subfunctionname, string? settingname, string? value, string? description, DateTime lastupdatedtimestamp, string? lastupdateduserid, long fk_mgau_id, string ENV)
        {
            ParameterSetting model = new ParameterSetting(id, functionname, subfunctionname, settingname, value, description, lastupdatedtimestamp, lastupdateduserid, fk_mgau_id, ENV);
            return model;
        }

        /// <summary>
        /// 設定尋條件
        /// </summary>
        public ParameterSetting SetQuery(string functionname, string subfunctionname, string settingname)
        {
            this.functionname = functionname;
            this.subfunctionname = subfunctionname;
            this.settingname = settingname;
            return this;
        }

        /// <summary>
        /// SetLastUpdatedTimestamp
        /// </summary>
        public ParameterSetting SetLastUpdatedTimestamp()
        {
            this.lastupdatedtimestamp = DateTime.Now;
            return this;
        }

        /// <summary>
        /// MGPS_SEQ
        /// </summary>
        public ParameterSetting SetID(long id)
        {
            this.id = id;
            return this;
        }

        /// <summary>
        /// 從FK_MGAU_ID取得Auth的名稱後Set
        /// </summary>
        public ParameterSetting SetAuth(string Auth)
        {
            this.Auth = Auth;
            return this;
        }
    }
}
