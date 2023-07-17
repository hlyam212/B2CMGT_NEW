using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using UtilityHelper;
using WebCommonHelper.Services.Authenticaiton;
using WebCommonHelper.Services.CallApi;
using EVABMS.AP.ConnectingString.Domain.Entities;

namespace EVABMS_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectingStringQueryController : EVABMSBase
    {
        public ConnectingStringQueryController(IUserService _userService, IConnect _connect)
        {
            this.userService = _userService;
            this.connect = _connect;
        }

        #region [HttpGet("FileName")] FileName
        [HttpGet]
        [Route("FileName")]
        public async Task<ApiResult<List<string>>> FileName()
        {
            try
            {
                string filesname = await connect.Get("", "ConnectingString/FileName");
                ApiResult<List<string>> files = JsonConvert.DeserializeObject<ApiResult<List<string>>>(filesname);
                return files;
            }
            catch (Exception e)
            {
                return new ApiError<List<string>>("EX", e.Message);
            }
        }
        #endregion

        #region [HttpGet("FileData/{fileName}")] FileData
        [HttpGet("FileData/{fileName}")]
        public async Task<ApiResult<List<ConnectingStringQuery>>> FileData(string? fileName = null)
        {
            try
            {
                string WBSResultJson = await connect.Get("", $"ConnectingString/FileData/{fileName}");
                ApiResult<List<ConnectingStringQuery>> result = JsonConvert.DeserializeObject<ApiResult<List<ConnectingStringQuery>>>(WBSResultJson);
                return result;
            }
            catch (Exception e)
            {
                return new ApiError<List<ConnectingStringQuery>>("EX", e.Message);
            }
        }
        #endregion

        #region [HttpPost("FileData")] FileDataUpload
        [HttpPost("FileData")]
        public async Task<ApiResult<List<ConnectingStringQuery>>> FileDataUpload(JsonObject input)
        {
            try
            {
                string WBSResultJson = await connect.Post(input, $"ConnectingString/FileData");
                ApiResult<List<ConnectingStringQuery>> result = JsonConvert.DeserializeObject<ApiResult<List<ConnectingStringQuery>>>(WBSResultJson);
                return result;
            }
            catch (Exception e)
            {
                return new ApiError<List<ConnectingStringQuery>>("EX", e.Message);
            }
        }
        #endregion

        #region [HttpPost("Update")] Update
        [HttpPost("Update")]
        public async Task<ApiResult<bool>> Update(JsonObject input)
        {
            ApiResult<bool> result = new();
            try
            {
                string WBSResultJson = await connect.Post(input, "ConnectingString/Update");
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
        #endregion
    }
}
