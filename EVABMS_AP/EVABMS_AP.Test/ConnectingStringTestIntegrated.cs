using EVABMS_AP.Test.Base;
using FluentAssertions;
using Newtonsoft.Json;
using UtilityHelper;

namespace B2CMGT_NEW.Test
{
    [TestClass]
    public class ConnectingStringTestIntegrated : IntegratedTesting
    {
        [TestMethod]
        public void IsAuthToFunction_Success()
        {
            Task<string> WBSResultJson = connect.Get("", $"User/IsAuthToFunction/{"E73970"}/{"FullControl"}");

            ApiResult<bool> result = JsonConvert.DeserializeObject<ApiResult<bool>>(WBSResultJson.Result);
            result.Succ.Should().BeTrue();
            result.Data.Should().BeTrue();
        }
       
    }
}