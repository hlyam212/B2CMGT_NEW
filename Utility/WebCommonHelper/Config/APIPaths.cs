using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommonHelper.Config
{
    public class APISettings
    {
        public APIOptions Api { get; set; }

        public class APIOptions 
        {
            public bool doEncryptAndDecrypt { get; set; }
            public string ip { get; set; }
            public string key { get; set; }
            public string salt { get; set; }
        }
    }
}
