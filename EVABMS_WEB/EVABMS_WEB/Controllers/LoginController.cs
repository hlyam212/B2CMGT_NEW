using EVABMS_AP.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UtilityHelper;
using WebCommonHelper.Entities.Enum;
using WebCommonHelper.Models;
using WebCommonHelper.Services.Authenticaiton;
using WebCommonHelper.Services.CallApi;

namespace EVABMS_WEB.Controllers
{
    [EnableCors(policyName)]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        public IConnect connect;
        public IUserService userService;
        public const string policyName = "EVABMS_WEB_POLICY";

        public LoginController(IUserService _userService, IConnect _connect)
        {
            this.userService = _userService;
            this.connect = _connect;
        }

        [HttpGet]
        [Route("AAMSLinkGet")]
        public async Task<string> AAMSLinkGet()
        {
            string result = "";
            ApiResult<List<ParameterSettingDataModel>> api_result = new ApiResult<List<ParameterSettingDataModel>>();
            try
            {
                string WBSResultJson = await connect.Post<ParameterSettingDataModel>(new ParameterSettingDataModel
                {
                    functionname = "EVABMS",
                    settingname = "AAMSLink"
                }, "ParameterSetting/Query");

                api_result = JsonConvert.DeserializeObject<ApiResult<List<ParameterSettingDataModel>>>(WBSResultJson);
                if (api_result.Succ == false) return result;
                if (api_result.Data.IsNullOrEmpty()) return result;
                result = api_result.Data.First().value;
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }

        [HttpPost("Query")]
        public async Task<ApiResult<object>> Query(AuthenticateRequest input)
        {
            ApiResult<object> result = new ApiResult<object>();
            try
            {
                WebCommonHelper.Entities.User userInfo = new WebCommonHelper.Entities.User();
                List<WebCommonHelper.Entities.User> testAccount = new List<WebCommonHelper.Entities.User> { new WebCommonHelper.Entities.User
                {
                     Company=Company.EVA,
                    UserId="TESTDIP",
                     FullName ="TESTDIP",
                     FirstName ="TESTDIP",
                     LastName  ="TESTDIP",
                     CompanyName="Test",
                     OfficeCode="Test",
                     JobTitle="Test",
                     Mail="testdip@evaair.com"
                } };
                userInfo = testAccount.FirstOrDefault(x => x.UserId == input.UserAccount);
                if (userInfo == null)
                {
                    userInfo = userService.Login(input);
                }

                #region 驗證AD
                AuthenticateResponse auth = userService.Authenticate(userInfo);
                if (auth == null)
                {
                    throw new UnauthorizedAccessException("Login Error. Please try again.");
                }
                #endregion

                #region 驗證授權
                string WBSResultJson = await connect.Get("", $"User/UserRole/{input.UserAccount}");

                ApiResult<List<string>> userRoles = JsonConvert.DeserializeObject<ApiResult<List<string>>>(WBSResultJson);
                if (userRoles.Succ == false || userRoles.Data.IsNullOrEmpty())
                {
                    throw new UnauthorizedAccessException("Unauthrized User. Please apply from AAMS.");
                }
                #endregion

                #region Log登入時間
                WBSResultJson = await connect.Post(input, $"User/LogLogin?userID={input.UserAccount}");
                JsonConvert.DeserializeObject<ApiResult<bool>>(WBSResultJson);
                #endregion

                userService.SetTokenCookie(Response.Cookies, auth.RefreshToken);

                result.Succ = true;
                result.Data = new
                {
                    auth = auth,
                    userRoles = userRoles.Data
                };
                return result;
            }
            catch (Exception ex)
            {
                result = new ApiError<object>("EX", ex.Message);
            }

            return result;
        }

    }
}
