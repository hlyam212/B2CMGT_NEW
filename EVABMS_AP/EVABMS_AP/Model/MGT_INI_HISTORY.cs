using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVABMS_AP.Model
{
    public class MGT_INI_HISTORY
    {
        public long ID { get; set; }
        public string FILE_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime LAST_UPDATED_TIMESTAMP { get; set; }
        public string LAST_UPDATED_USERID { get; set; }
    }
}
