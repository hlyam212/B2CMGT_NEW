using OracleAttribute.Attributes;
using Newtonsoft.Json;

namespace EVABMS.AP.Authorization.Domain.Entities
{
    public class AuthorizationModel
    {
        public string id { get; private set; }
        public string name { get; private set; }
        public AuthSetting? setting { get; private set; }
        public List<AuthorizationModel>? children { get; private set; }
        public List<AuthTo>? authto { get; private set; }
        public List<AuthSettingHistory>? settinghistory { get; private set; }

        public AuthorizationModel() { }

        [JsonConstructor]
        public AuthorizationModel(AuthSetting? Setting,
                                  List<AuthorizationModel>? Children,
                                  List<AuthTo>? AuthTo,
                                  List<AuthSettingHistory>? SettingHistory)
        {
            this.id = Setting.id.ToString();
            this.name = Setting.name;
            this.setting = Setting;
            this.children = Children;
            this.authto = AuthTo;
            this.settinghistory = SettingHistory;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static AuthorizationModel Create(AuthSetting Setting)
        {
            AuthorizationModel model = new AuthorizationModel(Setting, null, null, null);
            return model;
        }

        /// <summary>
        /// Create
        /// </summary>
        public static AuthorizationModel Create(AuthSetting Setting,
                                                List<AuthorizationModel>? Children,
                                                List<AuthTo>? AuthTo,
                                                List<AuthSettingHistory>? SettingHistory)
        {
            AuthorizationModel model = new AuthorizationModel(Setting, Children, AuthTo, SettingHistory);
            return model;
        }

        /// <summary>
        /// Create
        /// </summary>
        public AuthorizationModel SetChildren(List<AuthorizationModel>? _children)
        {
            this.children = _children.Where(x => x.setting.fk_mgau_id == this.setting.id).ToList();
            return this;
        }
        /// <summary>
        /// Create
        /// </summary>
        public AuthorizationModel SetAuthTo(List<AuthTo>? authorizedto)
        {
            this.authto = authorizedto.Where(x => x.fk_mgau_id == this.setting.id).ToList();
            return this;
        }
        /// <summary>
        /// Create
        /// </summary>
        public AuthorizationModel SetHistory(List<AuthSettingHistory>? authorsethistory)
        {
            this.settinghistory = authorsethistory.Where(x => x.fk_mgau_id == this.setting.id).ToList();
            return this;
        }
    }
}
