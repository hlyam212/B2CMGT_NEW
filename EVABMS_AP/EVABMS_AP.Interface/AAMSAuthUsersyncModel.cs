using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVABMS_AP.Interface
{
    public class AAMSAuthUsersyncModel
    {
        public string? UnitCode { get; set; }
        public string? SysCode { get; set; }
        public List<USERLIST>? UserList { get; set; }
        public class USERLIST
        {
            public string ADAccount { get; set; }
            public string Kind { get; set; }
            public string Code { get; set; }
        }
    }
}
