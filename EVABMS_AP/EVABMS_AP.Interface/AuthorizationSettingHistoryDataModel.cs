using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVABMS_AP.Interface
{
    public class AuthorizationSettingHistoryDataModel
    {
        public long id { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public long levels { get; set; }
        public string? action { get; set; }
        public DateTime lastupdatedtimestamp { get; set; }
        public string lastupdateduserid { get; set; }
        public long? fkmgauid { get; set; }
    }
}
