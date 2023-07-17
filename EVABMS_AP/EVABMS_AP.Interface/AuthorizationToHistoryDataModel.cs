using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVABMS_AP.Interface
{
    public class AuthorizationToHistoryDataModel
    {
        public long id { get; set; }
        public string department { get; set; }
        public string userid { get; set; }
        public string user_role { get; set; }
        public string action { get; set; }
        public DateTime effective_start { get; set; }
        public DateTime effective_end { get; set; }
        public DateTime last_updated_timestamp { get; set; }
        public string last_updated_userid { get; set; }
        public long fk_mgau_id { get; set; }
    }
}
