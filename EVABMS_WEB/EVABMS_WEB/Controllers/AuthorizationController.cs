using EVABMS.AP.Authorization.Domain.Entities;
using EVABMS_AP.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UtilityHelper;
using WebCommonHelper;
using WebCommonHelper.Services.Authenticaiton;
using WebCommonHelper.Services.CallApi;

namespace EVABMS_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : EVABMSBase
    {
        public AuthorizationController(IUserService _userService, IConnect _connect)
        {
            this.userService = _userService;
            this.connect = _connect;
        }

        [HttpGet]
        public async Task<ApiResult<List<AuthorizationModel>>> Query()
        {
            ApiResult<List<AuthorizationModel>> result = new ApiResult<List<AuthorizationModel>>();
            try
            {
                string WBSResultJson = await connect.Get("", "Authorization");

                result = JsonConvert.DeserializeObject<ApiResult<List<AuthorizationModel>>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<List<AuthorizationModel>>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        [HttpGet]
        [Route("{userid}")]
        public async Task<string> Query(string userid)
        {
            ApiResult<List<AuthorizationModel>> result = new ApiResult<List<AuthorizationModel>>();
            try
            {
                string WBSResultJson = await connect.Get("", $"Authorization/{userid}");

                //result = JsonConvert.DeserializeObject<ApiResult<List<AuthorizationModel>>>(WBSResultJson);
                return WBSResultJson;
            }
            catch (Exception ex)
            {
                //result = new ApiError<List<AuthorizationModel>>("EX", ex.Message + "\r\n" + ex.StackTrace);
                return JsonConvert.SerializeObject(new ApiError<List<AuthorizationModel>>("EX", ex.Message + "\r\n" + ex.StackTrace));
            }
            //return result;
        }

        /// <summary>
        /// 依照搜尋條件找出Authorization Setting
        /// </summary>
        /// <param userid="H39232">Search Criteria</param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpGet]
        [Route("QueryANode/{functionName}")]
        public async Task<ApiResult<AuthorizationDataModel>> QueryANode(string functionName)
        {
            ApiResult<AuthorizationDataModel> result = new ApiResult<AuthorizationDataModel>();
            try
            {
                string WBSResultJson = await connect.Get("", $"Authorization/QueryANode/{functionName}");

                result = JsonConvert.DeserializeObject<ApiResult<AuthorizationDataModel>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<AuthorizationDataModel>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        [HttpPost("UserRoleGet")]
        public async Task<ApiResult<List<string>>> UserRoleGet()
        {
            ApiResult<List<string>> result = new ApiResult<List<string>>();
            try
            {
                string WBSResultJson = await connect.Get("", "UserRole/Roles");

                result = JsonConvert.DeserializeObject<ApiResult<List<string>>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<List<string>>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        [HttpPost("Update")]
        public async Task<ApiResult<bool>> Update(AuthorizationDataModel dataModel)
        {
            ApiResult<bool> result = new ApiResult<bool>();
            try
            {
                string WBSResultJson = await connect.Post<AuthorizationDataModel>(dataModel, "Authorization/Update");
                if (WBSResultJson.IsNullOrEmpty())
                {
                    return result;
                }

                result = JsonConvert.DeserializeObject<ApiResult<bool>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<bool>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        [HttpPost("Delete")]
        public async Task<ApiResult<bool>> Delete(AuthorizationDataModel dataModel)
        {
            ApiResult<bool> result = new ApiResult<bool>();
            try
            {
                string WBSResultJson = await connect.Post<AuthorizationDataModel>(dataModel, "Authorization/Delete");
                if (WBSResultJson.IsNullOrEmpty())
                {
                    return result;
                }

                result = JsonConvert.DeserializeObject<ApiResult<bool>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<bool>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }
    }
}
