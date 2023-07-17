using EVABMS.AP.Authorization.Domain.Entities;
using EVABMS.AP.Parameter.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using UtilityHelper;
using WebCommonHelper.Services.Authenticaiton;
using WebCommonHelper.Services.CallApi;


namespace EVABMS_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParameterSettingController : EVABMSBase
    {
        public ParameterSettingController(IUserService _userService, IConnect _connect)
        {
            this.userService = _userService;
            this.connect = _connect;
        }

        [HttpPost("Query")]
        public async Task<ApiResult<List<ParameterSetting>>> Query(JsonObject searchModel)
        {
            ApiResult<List<ParameterSetting>> result = new ApiResult<List<ParameterSetting>>();
            try
            {
                string WBSResultJson = await connect.Post<object>(searchModel, "ParameterSetting/Query");
                result = JsonConvert.DeserializeObject<ApiResult<List<ParameterSetting>>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<List<ParameterSetting>>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        [HttpGet("Auth/{userId}")]
        public async Task<ApiResult<object>> QueryAuth(string userId)
        {
            ApiResult<object> result = new ApiResult<object>();
            try
            {
                string WBSResultJson = await connect.Get("", $"ParameterSetting/Auth/{userId}");
                ApiResult<string> authResult = JsonConvert.DeserializeObject<ApiResult<string>>(WBSResultJson);
                if (authResult.Succ == false)
                {
                    return result = new ApiError<object>("EX", authResult.Message);
                }

                WBSResultJson = await connect.Get("", $"Authorization/QueryANode/{"CPU.ParameterSetting"}");
                ApiResult<AuthorizationModel> ddlResult = JsonConvert.DeserializeObject<ApiResult<AuthorizationModel>>(WBSResultJson);
                if (ddlResult.Succ == false)
                {
                    return result = new ApiError<object>("EX", ddlResult.Message);
                }

                result = new ApiResult<object>(new
                {
                    auth = authResult.Data,
                    ddl = ddlResult.Data.children
                });
            }
            catch (Exception ex)
            {
                result = new ApiError<object>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        [HttpPost("Update")]
        public async Task<ApiResult<bool>> Update(JsonObject dataModel)
        {
            ApiResult<bool> result = new ApiResult<bool>();
            try
            {
                dataModel["lastupdateduserid"] = userService.GetUser().UserId;

                string WBSResultJson = await connect.Post<JsonObject>(dataModel, "ParameterSetting/Update");
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
        public async Task<ApiResult<bool>> Delete(JsonObject dataModel)
        {
            ApiResult<bool> result = new ApiResult<bool>();
            try
            {
                dataModel["lastupdateduserid"] = userService.GetUser().UserId;

                string WBSResultJson = await connect.Post<JsonObject>(dataModel, "ParameterSetting/Delete");
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

        [HttpPost("HistoryQuery")]
        public async Task<ApiResult<List<History>>> HistoryQuery(JsonObject searchModel)
        {
            ApiResult<List<History>> result = new ApiResult<List<History>>();
            try
            {
                string WBSResultJson = await connect.Post<JsonObject>(searchModel, "ParameterSetting/HistoryQuery");

                result = JsonConvert.DeserializeObject<ApiResult<List<History>>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<List<History>>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }
    }
}