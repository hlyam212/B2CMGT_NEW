using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommonHelper.Entities
{
    public class Application
    {
        public string appId { get; init; }
        public string appNo { get; set; }
        public string appInfo { get; set; }

        public Application() { }

        public Application(string orderId)
        {
            this.appId = orderId;
        }

        public Application(string appId, string appNo, string appInfo)
        {
            this.appId = appId;
            this.appNo = appNo;
            this.appInfo = appInfo;
        }

        public bool Exist()
        {
            return !string.IsNullOrEmpty(appNo) &&
                !string.IsNullOrEmpty(appInfo);
        }
    }
}
