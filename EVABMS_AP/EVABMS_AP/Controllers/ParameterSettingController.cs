using Asp.Versioning;
using EVABMS.AP.Authorization.Domain.Entities;
using EVABMS.AP.Authorization.Infrastructure;
using EVABMS.AP.Parameter.Domain.Entities;
using EVABMS.AP.Parameter.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OracleHelper.TransactSql;
using System.Data;
using System.Text.Json.Nodes;
using System.Transactions;
using UtilityHelper;

namespace EVABMS_AP.Controllers
{
    /// <summary>
    /// ParameterSettingController
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class ParameterSettingController : ControllerBase
    {
        /// <summary>
        /// 依照搜尋條件找出Parameter Setting
        /// </summary>
        /// <param name="queryData">Search Criteria</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]
        [HttpPost("Query")]
        public ApiResult<List<ParameterSetting>> Query(JsonObject queryData)
        {
            ApiResult<List<ParameterSetting>> result;

            List<ParameterSetting> dataModel = new List<ParameterSetting>();
            try
            {
                #region 先讀出指定的ParameterSetting
                ParameterSetting quitirua = new ParameterSetting().SetQuery(queryData["functionname"]?.ToString(), 
                                                                            queryData["subfunctionname"]?.ToString(),
                                                                            queryData["settingname"]?.ToString());
                dataModel = new ParameterRepository().Query(quitirua);

                AuthorizationModel funcNode = new AuthorizationRepository().QueryANode("CPU.ParameterSetting");
                if (funcNode == null)
                {
                    return new ApiError<List<ParameterSetting>>("500", "Authorization is missing.");
                }
                #endregion

                #region 權限管理
                string Auth = queryData["Auth"]?.ToString();
                List<string> authDefine = new List<string> { "Modify", "ReadOnly" };
                if (authDefine.Contains(Auth))
                {
                    List<long> authList = Auth == "Modify"
                                          ? funcNode.children.Where(x => authDefine.Contains(x.setting.name)).Select(x => x.setting.id).ToList()
                                          : funcNode.children.Where(x => x.setting.name == "ReadOnly").Select(x => x.setting.id).ToList();

                    dataModel = dataModel.Where(x => authList.Contains(x.fk_mgau_id)).ToList();
                    
                }
                dataModel.ForEach(x => 
                { if (x.fk_mgau_id > 0) x.SetAuth(funcNode.children.First(y => y.setting.id == x.fk_mgau_id).setting.name); });
                #endregion

                result = new ApiResult<List<ParameterSetting>>(dataModel);
                result.Message = "Query Success";
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<List<ParameterSetting>>("Exception", ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 依照AD撈出使用者在Parameter Setting中的權限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("Auth/{userId}")]
        public ApiResult<string> QueryAuth(string userId)
        {
            ApiResult<string> result = new ApiResult<string>();
            try
            {
                string auth = "CPU.ParameterSetting.FullControl";
                bool response = new UserRepository().IsAuthToFunction(userId, auth);
                if (response)
                {
                    return result = new ApiResult<string>(auth);
                }

                auth = "CPU.ParameterSetting.Modify";
                response = new UserRepository().IsAuthToFunction(userId, auth);
                if (response)
                {
                    return result = new ApiResult<string>(auth);
                }

                auth = "CPU.ParameterSetting.ReadOnly";
                response = new UserRepository().IsAuthToFunction(userId, auth);
                if (response)
                {
                    return result = new ApiResult<string>(auth);
                }

                return result = new ApiError<string>("500", $"You are not authrized to {"Parameter Setting"}");
            }
            catch (Exception ex)
            {
                result = new ApiError<string>("EX", ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// 新增/修改Parameter Setting
        /// </summary>
        /// <param name="insertJson"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public ApiResult<bool> Update(JsonObject insertJson)
        {
            ApiResult<bool> apiResult = new ApiResult<bool>();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    OraDataService ora = new OraDataService();

                    //先撈DB，要判斷是否有重複值
                    ParameterSetting insertData = JsonConvert.DeserializeObject<ParameterSetting>(insertJson.ToSafeString());

                    List<ParameterSetting> queryDB = new ParameterRepository().Query(insertData);

                    #region Insert
                    if (insertData.id == 0)
                    {
                        #region 先檢查有沒有重複值
                        if (queryDB.HasValue())
                        {
                            return apiResult = new ApiError<bool>("500", $"This setting is already exist. [{insertData.functionname}/{insertData.subfunctionname}/{insertData.settingname}]");
                        }
                        #endregion
                        apiResult.Succ = new ParameterRepository().Insert(insertData) != null;
                        if (apiResult.Succ == false)
                        {
                            return apiResult = new ApiError<bool>("500", "Insert MGT_PARAMETER_SETTINGS failed.");
                        }

                        apiResult.Message = "Insert success.";
                    }
                    #endregion
                    #region Update
                    else
                    {
                        #region 先檢查有沒有重複值
                        if (queryDB.Where(x => x.id != insertData.id).HasValue())
                        {
                            return apiResult = new ApiError<bool>("500", $"This setting is already exist. [{insertData.functionname}/{insertData.subfunctionname}/{insertData.settingname}]");
                        }
                        ParameterSetting org = queryDB.FirstOrDefault(x => x.id == insertData.id);
                        #endregion

                        apiResult.Succ = new ParameterRepository().Update(insertData, org) != null;

                        if (apiResult.Succ == false)
                        {
                            return apiResult = new ApiError<bool>("500", "Update MGT_PARAMETER_SETTINGS failed.");

                        }

                        apiResult.Message = "Update success.";
                    }
                    #endregion

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                apiResult = new ApiError<bool>("Exception", ex.Message);
            }
            return apiResult;
        }

        /// <summary>
        /// 刪除Parameter Setting
        /// </summary>
        /// <param name="insertJson"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public ApiResult<bool> Delete(JsonObject insertJson)
        {
            ApiResult<bool> apiResult = new ApiResult<bool>();
            try
            {
                ParameterSetting delData = JsonConvert.DeserializeObject<ParameterSetting>(insertJson.ToSafeString());
                apiResult.Succ = new ParameterRepository().Delete(delData) == 1;
                if (apiResult.Succ == false)
                {
                    apiResult.Message = "Insert MGT_PARAMETER_SETTINGS_HISTORY failed.";
                    return apiResult;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                apiResult = new ApiError<bool>("Exception", ex.Message);
            }
            return apiResult;
        }

        /// <summary>
        /// 依照搜尋條件找出Parameter Setting
        /// </summary>
        /// <param name="queryData">Search Criteria</param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpPost("HistoryQuery")]
        public ApiResult<List<History>> HistoryQuery(JsonObject queryData)
        {
            ApiResult<List<History>> result;

            List<History> dataModel = new List<History>();
            try
            {
                History quitirua = new History().SetQuery(queryData["functionname"]?.ToString(),
                                                          queryData["subfunctionname"]?.ToString(),
                                                          queryData["settingname"]?.ToString());
                dataModel = new ParameterRepository().QueryHistory(quitirua);
                result = new ApiResult<List<History>>(dataModel);
                result.Message = "Query Success";
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<List<History>>("Exception", ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 從Config倒資料進DB
        /// </summary>
        /// <param name="insertData"></param>
        /// <returns></returns>
        [HttpPost("DataConvert")]
        public ApiResult<bool> DataConvert(string configName = "web", string configURL = "", string func = "")
        {
            ApiResult<bool> apiResult = new ApiResult<bool>();
            //try
            //{
            //    if (configURL.IsNullOrEmpty())
            //    {
            //        configURL = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase; ;
            //    }
            //    var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = Path.Combine(configURL, configName + ".config") };
            //    var assemblyConfig = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            //    if (assemblyConfig.HasFile)
            //    {
            //        AppSettingsSection section = (assemblyConfig.GetSection("appSettings") as AppSettingsSection);
            //        foreach (var x in section.Settings.AllKeys)
            //        {
            //            try
            //            {
            //                string y = section.Settings[x].Value;
            //                Update(new ParameterSettingDataModel
            //                {
            //                    functionname = func,
            //                    settingname = x,
            //                    value = y,
            //                    lastupdateduserid = "BATCH"
            //                });
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.Write(ex.Message);
            //                continue;
            //            }
            //        }
            //    }
            //    apiResult.Succ = true;
            //}
            //catch (Exception ex)
            //{
            //    Console.Write(ex.Message);
            //    apiResult = new ApiError<bool>("Exception", ex.Message);
            //}
            return apiResult;
        }
    }
}