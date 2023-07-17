using EVABMS_AP.Interface;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using CommonHelper;
using EncryptionHelper;
using UtilityHelper;

namespace B2CMGT_NEW.Test
{
    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void TestRandom()
        {
            List<int> values = new List<int>() { 1, 2, 3, 4, 5 };

            Random rand = new Random();
            var shuffled = values.OrderBy(_ => rand.Next()).ToList();

            Console.WriteLine(String.Join(", ", shuffled));
        }
        [TestMethod]
        public void Test1Split()
        {
            ApiResult<bool> result = new ApiResult<bool>();
            //字符串集合
            List<string> list = new List<string>();
            list.Add("a");
            list.Add("");
            list.Add("c");
            list.Add("d");
            list.Add("e");
            string strTemp1 = string.Join(",", list.ToArray());
            result = new ApiError<bool>(null, strTemp1);
            Console.WriteLine(result.Message);
        }

        [TestMethod]
        public void TestEnc()
        {
            string a = EncryptionOrgService.GDPREncrypt("我是誰");

            Console.WriteLine(a);
        }
        [TestMethod]
        public void TestStrSplit() 
        {
            //string a = "";
            //int chunkSize = 3;
            //for (var i = 0; i<= 10; i++) 
            //{
            //    a += "1";
            //}
            //List<string> result = a.Chunk(chunkSize)
            //                       .Select(x => new string(x))
            //                       .ToList();
            //result.Count.Should().Be(4);
        }

        [TestMethod]
        public void ModelComparerTest()
        {
            List<AuthorizationDataModel> orgModel = new List<AuthorizationDataModel> { 
                new AuthorizationDataModel{id="1" },
                new AuthorizationDataModel{id="3" },
                new AuthorizationDataModel{id="4" }
            };
            List<AuthorizationDataModel> newModel = new List<AuthorizationDataModel>{
                new AuthorizationDataModel{id="2" },
                new AuthorizationDataModel{id="3",levels=1 },
            };
            ModelComparer<AuthorizationDataModel> compares = ModelComparer.Create(orgModel, newModel, m => m.id);
            compares.Insert.Count().Should().Be(1);
            compares.Update.Count().Should().Be(1);
            compares.Delete.Count().Should().Be(2);
        }

        public void ModelComparerTestt()
        {
            List<AuthorizationToDataModel> orgModel = new List<AuthorizationToDataModel> {
                new AuthorizationToDataModel{id=1 },
                new AuthorizationToDataModel{id=3 },
                new AuthorizationToDataModel{id=4 }
            };
            List<AuthorizationToDataModel> newModel = new List<AuthorizationToDataModel>{
                new AuthorizationToDataModel{id=2 },
                new AuthorizationToDataModel{id=3,fkmgauid=1 },
            };
            ModelComparer<AuthorizationToDataModel> compares = ModelComparer.Create(orgModel, newModel, m => m.id);
            compares.Insert.Count().Should().Be(1);
            compares.Update.Count().Should().Be(1);
            compares.Delete.Count().Should().Be(2);
        }

        [TestMethod]
        public void 讀取設定檔()
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json");
            var config = builder.Build();

            Console.WriteLine($"AppId = {config["AppId"]}");
            Console.WriteLine($"AppId = {config["Player:AppId"]}");
            Console.WriteLine($"Key = {config["Player:Key"]}");
            Console.WriteLine($"Connection String = {config["ConnectionStrings:DefaultConnectionString"]}");
        }

        [TestMethod]
        public void XXXX() 
        {
            string a = "E09D294ECF1F459BE0652B7F9C9984EFBD82ADE564D15BE56AA44F809A5C6159";

            string result= EncryptionOrgService.GDPRDecrypt(a);
        }
    }
}