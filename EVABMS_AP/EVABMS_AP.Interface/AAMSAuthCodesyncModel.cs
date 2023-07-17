using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVABMS_AP.Interface
{
    public class AAMSAuthCodesyncModel
    {
        public string? UnitCode { get; set; }
        public string? SysCode { get; set; }
        public List<CODELIST>? CodeList { get; set; }
        public class TEST
        {
            public string USER_ROLE { get; set; }
            public string DESCRIPTION { get; set; }
            public DateTime LAST_UPDATED_TIMESTAMP { get; set; }
            public string? LAST_UPDATED_USERID { get; set; }
        }
        public class CODELIST
        {
            public string Kind { get; set; }
            public string PCode { get; set; }
            public string ParentCode { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string Remark { get; set; }
        }
    }
}
