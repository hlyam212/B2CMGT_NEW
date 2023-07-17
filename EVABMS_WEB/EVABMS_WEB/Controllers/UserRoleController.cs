using EVABMS_AP.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UtilityHelper;
using WebCommonHelper.Services.Authenticaiton;
using WebCommonHelper.Services.CallApi;

namespace EVABMS_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : EVABMSBase
    {
        public UserRoleController(IUserService _userService, IConnect _connect)
        {
            this.userService = _userService;
            this.connect = _connect;
        }

        [HttpGet]
        public async Task<ApiResult<List<UserRoleDataModel>>> Query()
        {
            ApiResult<List<UserRoleDataModel>> result = new();
            try
            {
                string WBSResultJson = await connect.Get("", "UserRole");
                result = JsonConvert.DeserializeObject<ApiResult<List<UserRoleDataModel>>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<List<UserRoleDataModel>>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        [HttpPost("Update")]
        public async Task<ApiResult<bool>> Update(List<MGT_USER_ROLESDataModel> input)
        {
            ApiResult<bool> result = new();
            try
            {
                string WBSResultJson = await connect.Post(input, "UserRole/Update");
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
