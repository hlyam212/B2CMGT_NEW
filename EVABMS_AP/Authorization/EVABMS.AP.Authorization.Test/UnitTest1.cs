using EVABMS.AP.Authorization.Domain.Entities;
using Newtonsoft.Json;

namespace EVABMS.AP.Authorization.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            #region 假資料
            string data = "";
            #region MGT_AUTHORIZATION_SETTING
            data = $"[{{\"id\":341,\"function_name\":\"Wifi\",\"description\":\"Wifi Voucher的後台管理\",\"last_updated_timestamp\":\"2023\\/03\\/31 03:47:13\",\"last_updated_userid\":\"TESTDIP\",\"fk_mgau_id\":0,\"levels\":0,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":302,\"function_name\":\"Authorization\",\"description\":\"Authorization\",\"last_updated_timestamp\":\"2023\\/03\\/31 11:33:31\",\"last_updated_userid\":\"E73970\",\"fk_mgau_id\":24,\"levels\":1,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":366,\"function_name\":\"File Management\",\"description\":\"INI更新\",\"last_updated_timestamp\":\"2023\\/03\\/21 12:06:51\",\"last_updated_userid\":\"E73970\",\"fk_mgau_id\":362,\"levels\":2,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":6,\"function_name\":\"CPU\",\"description\":\"string\",\"last_updated_timestamp\":\"2022\\/12\\/28 08:24:08\",\"last_updated_userid\":\"E73970\",\"levels\":0,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":444,\"function_name\":\"RoleModify\",\"last_updated_timestamp\":\"2023\\/05\\/09 05:27:38\",\"last_updated_userid\":\"H40913\",\"fk_mgau_id\":314,\"levels\":2,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":370,\"function_name\":\"Query\",\"description\":\"讀取各版本的INI內容\",\"last_updated_timestamp\":\"2023\\/04\\/19 09:49:01\",\"last_updated_userid\":\"E73970\",\"fk_mgau_id\":362,\"levels\":2,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":350,\"function_name\":\"ReadOnly\",\"description\":\"ReadOnly\",\"last_updated_timestamp\":\"2023\\/03\\/27 04:05:18\",\"last_updated_userid\":\"E73970\",\"fk_mgau_id\":282,\"levels\":2,\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":314,\"function_name\":\"UserRole\",\"description\":\"UserRole\",\"last_updated_timestamp\":\"2023\\/05\\/09 04:45:00\",\"last_updated_userid\":\"E73970\",\"fk_mgau_id\":24,\"levels\":1,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":24,\"function_name\":\"Settings\",\"description\":\"string\",\"last_updated_timestamp\":\"2022\\/12\\/30 02:27:07\",\"last_updated_userid\":\"H39232\",\"fk_mgau_id\":0,\"levels\":0,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":362,\"function_name\":\"Connect String\",\"description\":\"Connection INI 的管理\",\"last_updated_timestamp\":\"2023\\/03\\/21 12:05:01\",\"last_updated_userid\":\"E73970\",\"fk_mgau_id\":6,\"levels\":1,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":271,\"function_name\":\"Lottery\",\"description\":\"Lottery\",\"last_updated_timestamp\":\"2023\\/04\\/20 03:32:26\",\"last_updated_userid\":\"H40913\",\"fk_mgau_id\":0,\"levels\":0,\"aams_sys_code\":\"XXXXXXX123\",\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":342,\"function_name\":\"FullControl\",\"description\":\"可以CRUD的最高權限\",\"last_updated_timestamp\":\"2023\\/03\\/20 11:36:12\",\"last_updated_userid\":\"E73970\",\"fk_mgau_id\":282,\"levels\":2,\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":282,\"function_name\":\"Parameter Setting\",\"description\":\"Parameter Setting\",\"last_updated_timestamp\":\"2023\\/03\\/31 10:16:43\",\"last_updated_userid\":\"E73970\",\"fk_mgau_id\":6,\"levels\":1,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":442,\"function_name\":\"RoleQuery\",\"last_updated_timestamp\":\"2023\\/05\\/09 05:27:29\",\"last_updated_userid\":\"H40913\",\"fk_mgau_id\":314,\"levels\":2,\"menu_ind\":\"Y\",\"env\":\"DVP\"}}\r\n," +
                    $"{{\"id\":346,\"function_name\":\"Modify\",\"description\":\"只能修改Value的權限\",\"last_updated_timestamp\":\"2023\\/03\\/20 11:51:16\",\"last_updated_userid\":\"E73970\",\"fk_mgau_id\":282,\"levels\":2,\"env\":\"DVP\"}}\r\n]";
            List<AuthSetting> MGT_AUTHORIZATION_SETTING = JsonConvert.DeserializeObject<List<AuthSetting>>(data);
            MGT_AUTHORIZATION_SETTING.GetType();
            #endregion
            #endregion
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}