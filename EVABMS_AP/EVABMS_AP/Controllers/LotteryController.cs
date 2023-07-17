using EVABMS.AP.Lottery.Domain.Entities;
using EVABMS.AP.Lottery.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Nodes;
using System.Transactions;
using UtilityHelper;


namespace EVABMS_AP.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class LotteryController : ControllerBase
    {
        #region [HttpGet] QueryData
        /// <summary>
        /// 找出全部的LotteryData
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult<List<LotteryData>> Query()
        {
            try
            {
                ApiResult<List<LotteryData>> result = new();
                LotteryRepository repository = new();
                List<LotteryData> data = repository.Query();

                return result = new ApiResult<List<LotteryData>>(data);
            }
            catch (Exception ex)
            {
                return new ApiError<List<LotteryData>>(null, ex.Message);
            }

        }
        #endregion

        #region [HttpGet][Route("{id}")] QuerySingleData by id
        /// <summary> 
        /// 依照搜尋條件找出LotteryData
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public ApiResult<LotteryModel> Query(long id)
        {
            ApiResult<LotteryModel> result = new ApiResult<LotteryModel>();
            LotteryRepository repository = new();
            try
            {
                //query_data
                LotteryData lotteryData = repository.QueryDatabyID(id);
                if (lotteryData == null) return new ApiError<LotteryModel>(null, "Data Not Found");
                //data_list
                List<LotteryName> lotteryNames = repository.QueryName(id);
                //data_prize
                List<LotteryPrize> lotteryPrizes = repository.QueryPrize(id);
                //winner_list
                List<LotteryName> winners = (from x in lotteryNames
                                             where x.fk_mlop_id > 0
                                             select x).ToList();
                //return value
                LotteryModel lotteryModel = LotteryModel.Create(lotteryData, winners, lotteryNames, lotteryPrizes);
                result = new ApiResult<LotteryModel>(lotteryModel);
            }
            catch (Exception e)
            {
                result = new ApiError<LotteryModel>(null, e.Message);
            }
            return result;
        }
        #endregion

        #region [HttpPost("Insert")] Insert MGT_LOTTERY_DATA
        /// <summary>
        /// 新增 LotteryData
        /// </summary>
        /// <param name="insertJson"></param>
        /// <returns></returns>
        [HttpPost("Insert")]
        public ApiResult<long> MGTLotteryInsert(JsonObject insertJson)
        {
            ApiResult<long> result = new();
            ApiResult<bool> result_list = new() { Succ = true };
            ApiResult<bool> result_prize = new() { Succ = true };
            LotteryRepository repository = new();
            LotteryModel input = JsonConvert.DeserializeObject<LotteryModel>(insertJson.ToSafeString());
            List<string> file_list = JsonConvert.DeserializeObject<List<string>>(insertJson["file_list"].ToSafeString());
            List<string> file_prize = JsonConvert.DeserializeObject<List<string>>(insertJson["file_prize"].ToSafeString());
            try
            {

                //Add MGT_LOTTERY_DATA
                if (input.query_data.id == 0)
                {
                    result.Succ = repository.DataInsert(input.query_data) != 0;
                    if (!result.Succ) return new ApiError<long>(null, "LOTTERY_DATA table insert failed");
                }
                else
                {
                    result.Succ = repository.DataUpdate(input.query_data,
                                                        repository.QueryDatabyID(input.query_data.id)) == 1;
                }

                if (!file_list.IsNullOrEmpty()) result_list.Succ = repository.NameInsert(file_list,
                                                                                         input.query_data.id,
                                                                                         input.query_data.last_updated_userid);

                if (!file_prize.IsNullOrEmpty()) result_prize.Succ = repository.PrizeInsert(file_prize,
                                                                                            input.query_data.id,
                                                                                            input.query_data.last_updated_userid);

                if (result.Succ && result_list.Succ && result_prize.Succ)
                {
                    result = new ApiResult<long>
                    {
                        Message = "Insert success.",
                        Data = input.query_data.id,
                        Succ = result.Succ,
                    };
                }
                else
                {
                    result = new ApiError<long>(null, "Insert failed.");
                    return result;
                }
            }
            catch (Exception e)
            {
                result = new ApiError<long>(null, e.Message);
            }
            return result;
        }
        #endregion

        #region [HttpPost("List/Insert")] Insert nameList
        /// <summary>
        /// 新增 LotteryListData
        /// </summary>
        /// <param name="insertJson"></param>
        /// <returns></returns>
        [HttpPost("List/Insert")]
        public ApiResult<bool> ListInsert(JsonObject insertJson)
        {
            ApiResult<bool> result = new();
            LotteryRepository repository = new();
            LotteryModel input = JsonConvert.DeserializeObject<LotteryModel>(insertJson.ToSafeString());
            List<string> file_list = JsonConvert.DeserializeObject<List<string>>(insertJson["file_list"].ToSafeString());
            try
            {
                //處理傳入的List, 第一行是欄位, 其餘是名單資訊, 用換行分開
                if (file_list.IsNullOrEmpty())
                {
                    result = new ApiError<bool>(null, "沒有傳入人員清單");
                    return result;
                }

                if (file_list.Count < 2)
                {
                    result = new ApiError<bool>(null, "輸入資料筆數有誤");
                    return result;
                }

                #region 更新MGT_LOTTERY_DATA內LIST_COLUMN
                LotteryData lotteryData = input.query_data;

                lotteryData.SetListColumn(file_list[0]);
                file_list.RemoveAt(0);
                lotteryData.SetLastUpdatedTimestamp();
                lotteryData.SetLastUpdatedUserID(input.query_data.last_updated_userid);

                result.Succ = repository.DataUpdate(lotteryData,
                                                    repository.QueryDatabyID(input.query_data.id)) == 1;
                if (!result.Succ) return new ApiError<bool>(null, "更新DATA TABLE欄位錯誤");
                #endregion

                #region INSERT MGT_LOTTERY_LIST
                result.Succ = repository.NameInsert(file_list, input.query_data.id, input.query_data.last_updated_userid);
                if (!result.Succ) return new ApiError<bool>(null, "Insert List failed");
                #endregion

                result.Message = "Insert success.";
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<bool>("Exception", ex.Message);
            }

            return result;
        }
        #endregion

        #region [HttpPost("Prize/Insert")] Insert PrizeList
        /// <summary>
        /// 新增 LotteryPrizeData
        /// </summary>
        /// <param name="insertJson"></param>
        /// <returns></returns>
        [HttpPost("Prize/Insert")]
        public ApiResult<bool> PrizeInsert(JsonObject insertJson)
        {
            ApiResult<bool> result = new();
            LotteryRepository repository = new LotteryRepository();
            LotteryModel input = JsonConvert.DeserializeObject<LotteryModel>(insertJson.ToSafeString());
            List<string> file_prize = JsonConvert.DeserializeObject<List<string>>(insertJson["file_prize"].ToSafeString());
            try
            {
                result.Succ = repository.PrizeInsert(file_prize, input.query_data.id, input.query_data.last_updated_userid);
                if (!result.Succ) return new ApiError<bool>(null, "Insert prize failed");

                result.Message = "Insert success.";
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<bool>("Exception", ex.Message);
            }

            return result;
        }
        #endregion

        #region [HttpGet("Delete/{id}")] Delete MGT_LOTTERY_DATA
        /// <summary>
        /// 刪除 LotteryData
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Delete/{id}")]
        public ApiResult<bool> MGTLotteryDelete(long id)
        {
            ApiResult<bool> result = new ApiResult<bool>();
            LotteryData lotteryData = new LotteryData();
            LotteryRepository repository = new LotteryRepository();

            try
            {
                result.Succ = repository.DeleteData(lotteryData, id) == 1;
                result.Message = "Delete MGT_LOTTERY_DATA success.";

                if (!result.Succ)
                {
                    result = new ApiError<bool>(null, "Delete MGT_LOTTERY_DATA failed.");
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<bool>("Exception", ex.Message);
            }

            return result;
        }
        #endregion

        #region [HttpPost("List/Delete")] Delete nameList
        /// <summary>
        /// 刪除 LotteryListData
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("List/Delete/{id}")]
        public ApiResult<int> ListDelete(long id)
        {
            ApiResult<int> result = new ApiResult<int>();
            LotteryName lotteryName = new LotteryName();
            LotteryRepository repository = new LotteryRepository();
            try
            {
                int i = repository.DeleteName(lotteryName, id);
                result = new ApiResult<int>(i)
                {
                    Succ = true,
                    Message = @$"Delete List success.{i} record."
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<int>("Exception", ex.Message);
            }

            return result;
        }
        #endregion

        #region [HttpPost("Prize/Delete")] Delete PrizeList
        /// <summary>
        /// 刪除 LotteryPrizeData
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Prize/Delete/{id}")]
        public ApiResult<int> PrizeDelete(long id)
        {
            ApiResult<int> result = new ApiResult<int>();
            LotteryPrize lotteryPrize = new LotteryPrize();
            LotteryRepository repository = new LotteryRepository();

            try
            {
                int i = repository.DeletePrize(lotteryPrize, id);
                result = new ApiResult<int>(i)
                {
                    Succ = true,
                    Message = @$"Delete List success.{i} record."
                };

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<int>("Exception", ex.Message);
            }

            return result;
        }
        #endregion

        #region [HttpPost("Draw")] Draw
        /// <summary>
        /// 抽獎
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Draw/{id}")]
        public ApiResult<bool> Draw(long id)
        {
            ApiResult<bool> result = new();
            LotteryRepository repository = new LotteryRepository();

            try
            {
                //撈出抽獎人員亂數排列
                Random rand = new Random();
                List<LotteryName> namelist = repository.QueryName(id, false);
                List<LotteryName> shuffled = namelist.OrderBy(_ => rand.Next()).ToList();

                //撈出獎品
                List<LotteryPrize> prizelist = repository.QueryPrize(id);

                //抽獎
                long total = prizelist.Sum(t => t.numbers);
                if (total > shuffled.Count) return new ApiError<bool>(null, "獎品數大於抽獎人數...");

                foreach (LotteryPrize x in prizelist)
                {
                    for (long i = x.numbers; i > 0; i--)
                    {
                        using (TransactionScope ts = new())
                        {
                            int index = rand.Next(shuffled.Count);
                            LotteryName winner = shuffled[index];
                            winner.SetWinning(x.id);
                            result.Succ = repository.NameUpdate(winner, winner.id) == 1;
                            shuffled.RemoveAt(index);
                            ts.Complete();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = new ApiError<bool>("Exception", e.Message);
            }

            return result;
        }
        #endregion

    }
}
