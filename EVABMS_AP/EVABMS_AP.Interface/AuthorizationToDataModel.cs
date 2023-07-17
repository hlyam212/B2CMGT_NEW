using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVABMS_AP.Interface
{
    public class AuthorizationToDataModel
    {
        public long id { get; set; }
        public DateTime lastupdatedtimestamp { get; set; }
        public string? lastupdateduserid { get; set; }
        public long fkmgauid { get; set; }
        public string? effectivestart { get; set; }
        public string? effectiveend { get; set; }
        public DateTime effective_dt_start { get; set; }
        public DateTime effective_dt_end { get; set; }
        public bool? state { get; set; }
        public string? user_role { get; set; }
    }
}
