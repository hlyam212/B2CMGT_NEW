using OracleAttribute.Attributes;
using Newtonsoft.Json;

namespace EVABMS.AP.Authorization.Domain.Entities
{
    /// <summary>
    /// MGT_AUTHORIZATION_SETTING
    /// </summary>
    [TableName("MGT_AUTHORIZATION_SETTING")]
    public class AuthSetting
    {
        /// <summary>
        /// MGAU_SEQ
        /// </summary>
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }
        /// <summary>
        /// 功能/權限的名稱
        /// </summary>
        [ColumnName("FUNCTION_NAME")]
        public string name { get; private set; }
        /// <summary>
        /// 說明
        /// </summary>
        [ColumnName("DESCRIPTION")]
        public string description { get; private set; }
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
        /// 這一筆功能/權限的Parent
        /// </summary>
        [ColumnName("FK_MGAU_ID")]
        public long fk_mgau_id { get; private set; }
        /// <summary>
        /// 在權限中屬於第幾層
        /// </summary>
        [ColumnName("LEVELS")]
        public int levels { get; private set; }
        /// <summary>
        /// AAMS_SYS_CODE
        /// </summary>
        [ColumnName("AAMS_SYS_CODE")]
        public string aamssyscode { get; private set; }
        /// <summary>
        /// 此筆功能/權限是否為網站的選單
        /// </summary>
        [ColumnName("MENU_IND")]
        public string menu { get; private set; }
        /// <summary>
        /// 判斷權限的完整權限值
        /// </summary>
        [ColumnName("AUTH", indexes: new int[] { 1 })]
        public string auth { get; private set; }
        /// <summary>
        /// DB的環境
        /// </summary>
        [ColumnName("ENV")]
        public string env { get; private set; }

        public AuthSetting() { }

        [JsonConstructor]
        public AuthSetting(long id,
                           string name,
                           string description,
                           DateTime lastupdatedtimestamp,
                           string lastupdateduserid,
                           long fkmgauid,
                           string AAMSSysCode,
                           int levels,
                           string menu,
                           string Auth,
                           string ENV)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
            this.lastupdateduserid = lastupdateduserid;
            this.fk_mgau_id = fkmgauid;
            this.aamssyscode = AAMSSysCode;
            this.levels = levels;
            this.menu = menu;
            this.auth = Auth;
            this.env = ENV;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static AuthSetting Create(long id,
                                         string name,
                                         string description,
                                         DateTime lastupdatedtimestamp,
                                         string lastupdateduserid,
                                         long fkmgauid,
                                         string AAMSSysCode,
                                         int levels,
                                         string menu,
                                         string Auth,
                                         string ENV)
        {
            AuthSetting model = new AuthSetting(id, name, description, lastupdatedtimestamp, lastupdateduserid, fkmgauid, AAMSSysCode, levels, menu, Auth, ENV);
            return model;
        }

        /// <summary>
        /// SetAuth
        /// </summary>
        public AuthSetting SetAuth(string Auth)
        {
            this.auth = Auth;
            return this;
        }
    }
}
