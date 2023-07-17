using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVABMS_AP.Interface
{
    public class AAMSAuthResultModel
    {
        public string ResultCode { get; set; }
        public DateTime ReplyTime { get; set; }
        public string Msg { get; set; }
    }
}
