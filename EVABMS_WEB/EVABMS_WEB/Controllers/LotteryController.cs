using EVABMS.AP.Lottery.Domain.Entities;
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
    public class LotteryController : EVABMSBase
    {
        public LotteryController(IUserService _userService, IConnect _connect)
        {
            this.userService = _userService;
            this.connect = _connect;
        }

        [HttpGet]
        public async Task<ApiResult<List<LotteryData>>> Query()
        {
            ApiResult<List<LotteryData>> result = new ApiResult<List<LotteryData>>();
            try
            {
                string WBSResultJson = await connect.Get("", "Lottery");

                result = JsonConvert.DeserializeObject<ApiResult<List<LotteryData>>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<List<LotteryData>>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        [HttpPost("Update")]
        public async Task<ApiResult<long>> Update(JsonObject input)
        {
            ApiResult<long> result = new ApiResult<long>();
            try
            {

                string WBSResultJson = await connect.Post(input, "Lottery/Insert");

                result = JsonConvert.DeserializeObject<ApiResult<long>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<long>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }

            return result;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ApiResult<LotteryModel>> Queryone(long id)
        {
            ApiResult<LotteryModel> result = new ApiResult<LotteryModel>();
            try
            {
                string WBSResultJson = await connect.Get("", $"Lottery/{id}");

                result = JsonConvert.DeserializeObject<ApiResult<LotteryModel>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<LotteryModel>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        [HttpGet("Delete/{id}")]
        public async Task<ApiResult<bool>> Delete(long id)
        {
            ApiResult<bool> result = new();

            try
            {
                string WBSResultJson = await connect.Get("", $"Lottery/Delete/{id}");
                result = JsonConvert.DeserializeObject<ApiResult<bool>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<bool>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }

            return result;
        }

        [HttpPost("List/Insert")]
        public async Task<ApiResult<bool>> ListInsert(JsonObject insertJson)
        {
            ApiResult<bool> result = new();
            try
            {
                string WBSResultJson = await connect.Post(insertJson, "Lottery/List/Insert");
                result = JsonConvert.DeserializeObject<ApiResult<bool>>(WBSResultJson);
            }
            catch (Exception ex)
            {
                result = new ApiError<bool>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }

            return result;
        }

        [HttpPost("List/Delete/{id}")]
        public async Task<ApiResult<bool>> ListDelete(long id)
        {
            ApiResult<bool> result = new();
            try
            {
                string WBSResultJson = await connect.Post("", $"Lottery/List/Delete/{id}");
                result = JsonConvert.DeserializeObject<ApiResult<bool>>(WBSResultJson);

            }
            catch (Exception ex)
            {
                result = new ApiError<bool>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }

            return result;
        }

        [HttpPost("Prize/Insert")]
        public async Task<ApiResult<bool>> PrizeInsert(JsonObject insertJson)
        {
            ApiResult<bool> result = new();
            try
            {
                string WBSResultJson = await connect.Post(insertJson, "Lottery/prize/Insert");
                result = JsonConvert.DeserializeObject<ApiResult<bool>>(WBSResultJson);

            }
            catch (Exception ex)
            {
                result = new ApiError<bool>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }

            return result;
        }

        [HttpPost("Prize/Delete/{id}")]
        public async Task<ApiResult<bool>> PrizDelete(long id)
        {
            ApiResult<bool> result;
            try
            {
                string WBSResultJson = await connect.Post("", $"Lottery/Prize/Delete/{id}");
                result = JsonConvert.DeserializeObject<ApiResult<bool>>(WBSResultJson);

            }
            catch (Exception ex)
            {
                result = new ApiError<bool>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }

            return result;
        }


        [HttpPost("Draw/{id}")]
        public async Task<ApiResult<bool>> Draw(long id)
        {
            ApiResult<bool> result;
            try
            {
                string WBSResultJson = await connect.Post("", $"Lottery/Draw/{id}");
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
