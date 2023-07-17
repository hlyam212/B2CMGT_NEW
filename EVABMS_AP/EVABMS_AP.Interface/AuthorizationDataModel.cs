using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVABMS_AP.Interface
{
    public class AuthorizationDataModel
    {
        public string id { get; set; }
        public string? name { get; set; }
        public string? aams_syscode { get; set; }
        public string? description { get; set; }
        public DateTime lastupdatedtimestamp { get; set; }
        public string? lastupdateduserid { get; set; }
        public string? fkmgauid{ get; set; }
        public int levels { get; set; }
        public string? menu { get; set; }
        public List<AuthorizationDataModel>? children { get; set; }
        public List<AuthorizationToDataModel>? authorizedto { get; set; }
        public List<AuthorizationToDataModel>? newauthorizedto  { get; set; }
        public List<AuthorizationSettingHistoryDataModel>? authorsethistory { get; set; }
        public List<AuthorizationToHistoryDataModel>? auth_to_history { get; set; }
    }
}
