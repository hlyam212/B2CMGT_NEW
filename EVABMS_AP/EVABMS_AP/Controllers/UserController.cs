using EVABMS.AP.Authorization.Domain.Entities;
using EVABMS.AP.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using UtilityHelper;

namespace EVABMS_AP.Controllers
{
    /// <summary>
    /// User相關的WEB API
    /// </summary>
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// 記錄使用者的登入時間
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost("LogLogin")]
        public ApiResult<bool> LogLogin(string userID)
        {
            ApiResult<bool> apiResult = new ApiResult<bool>();
            try
            {

                User thisUser = new UserRepository().Query(userID);
                if (thisUser == null)
                {
                    return apiResult = new ApiError<bool>("500", "Can not find user in database");
                }

                User updateUser = EVABMS.AP.Authorization.Domain.Entities.User.Clone(thisUser);
                updateUser.SetLoginTimestamp();
                apiResult.Succ = new UserRepository().Update(updateUser, thisUser) == 1;
                if (apiResult.Succ == false)
                {
                    return apiResult = new ApiError<bool>("500", $"Update last logging timestamp failed.");
                }

                apiResult = new ApiResult<bool>(true);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                apiResult = new ApiError<bool>("Exception", ex.Message);
            }
            return apiResult;
        }

        /// <summary>
        /// 依照USERID找出有被授權的User Role
        /// </summary>
        /// <param userid="H39232">User ID</param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpGet]
        [Route("UserRole/{userid}")]
        public ApiResult<List<string>> QueryUserRole(string userID)
        {
            ApiResult<List<string>> result = new ApiResult<List<string>>();
            try
            {
                result = new ApiResult<List<string>>(new UserRepository().QueryUserRole(userID));
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<List<string>>("Exception", ex.Message);
            }
            return result;
        }
    }
}
