using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonHelper.Config;
using WebCommonHelper.Services.CallApi;

namespace EVABMS_AP.Test.Base
{
    public class IntegratedTesting
    {
        public IConnect connect;
        public IntegratedTesting()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                   .AddEnvironmentVariables()
                                                   .Build();
            IOptions<APISettings> myOptions = Options.Create(config.GetSection("APISettings").Get<APISettings>());
            connect = new CallAPI(myOptions);
        }
    }
}
