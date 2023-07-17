using CommonHelper;
using EVABMS.AP.Authorization.Domain.Entities;
using EVABMS.AP.Authorization.Infrastructure;
using EVABMS_AP.Interface;
using EVABMS_AP.Model;
using Microsoft.AspNetCore.Mvc;
using OracleHelper.TransactSql;
using System.Data;
using System.Transactions;
using UtilityHelper;

namespace EVABMS_AP.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]

    public class AuthorizationController : ControllerBase
    {
        /// <summary>
        /// 找出全部的Authorization Setting
        /// </summary>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpGet]
        public ApiResult<List<AuthorizationModel>> Query()
        {
            ApiResult<List<AuthorizationModel>> result;

            try
            {
                List<AuthorizationModel> dataModely = new AuthorizationRepository().Query();
                result = new ApiResult<List<AuthorizationModel>>(dataModely);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<List<AuthorizationModel>>("Exception", ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 依照USERID找出有被授權的Authorization Setting
        /// </summary>
        /// <param userid="H39232">User ID</param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpGet]
        [Route("{userid}")]
        public ApiResult<List<AuthorizationModel>> Query(string userid)
        {
            ApiResult<List<AuthorizationModel>> result = new ApiResult<List<AuthorizationModel>>();
            try
            {
                OraDataService ora = new OraDataService();

                #region Query MGT_AUTHORIZED_USERS
                List<string> authRoles = new UserRepository().QueryUserRole(userid);

                if (authRoles.IsNullOrEmpty())
                {
                    result = new ApiError<List<AuthorizationModel>>("500", $"AD:{userid} can not find user roles.");
                    return result;
                }

                if (authRoles.Contains("OMNISCIENT"))
                {
                    result = new ApiResult<List<AuthorizationModel>>(new AuthorizationRepository().Query());
                    return result;
                }
                #endregion

                result = new ApiResult<List<AuthorizationModel>>(new AuthorizationRepository().Query(authRoles,true));
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<List<AuthorizationModel>>("Exception", ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 依照USERID找出有被授權的Authorization Setting
        /// </summary>
        /// <param userid="H39232">User ID</param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpPost]
        [Route("QueryByRoles")]
        public ApiResult<List<AuthorizationDataModel>> Query(List<string> userRoles)
        {
            ApiResult<List<AuthorizationDataModel>> result = new ApiResult<List<AuthorizationDataModel>>();
            try
            {
                if (userRoles.IsNullOrEmpty())
                {
                    result = new ApiError<List<AuthorizationDataModel>>("500", $"Can not find user roles.");
                    return result;
                }

                OraDataService ora = new OraDataService();

                #region Query MGT_AUTHORIZATION_SETTING
                ora = new OraDataService();
                string sql = @" SELECT * FROM MGT_AUTHORIZATION_SETTING";
                List<MGT_AUTHORIZATION_SETTING> dbModel_AUTHORIZATION_SETTING = ora.Select<MGT_AUTHORIZATION_SETTING>(sql);
                #endregion

                #region Query MGT_AUTHORIZATION_TO
                ora = new OraDataService();
                sql = @$" SELECT * FROM MGT_AUTHORIZATION_TO WHERE USER_ROLE IN ({string.Join(',', userRoles.Select(x => $"'{x}'"))})";
                List<MGT_AUTHORIZATION_TO> dbModel_AUTHORIZATION_TO = ora.Select<MGT_AUTHORIZATION_TO>(sql);

                //只找出生效的
                dbModel_AUTHORIZATION_TO = dbModel_AUTHORIZATION_TO.Where(x => Extension.DateIntervalDuplicateCheck(x.EFFECTIVE_START, x.EFFECTIVE_END, DateTime.Now, null)).ToList();
                #endregion

                List<AuthorizationDataModel> dataModely = AuthSettingTreeBuild(dbModel_AUTHORIZATION_SETTING, dbModel_AUTHORIZATION_TO);
                result = new ApiResult<List<AuthorizationDataModel>>(dataModely);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<List<AuthorizationDataModel>>("Exception", ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 傳入Function Name, 找出這一個和下一層的Authorization Setting 和 Authorization To
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("QueryANode/{functionName}")]
        public ApiResult<AuthorizationModel> QueryANode(string functionName)
        {
            AuthorizationModel result = new AuthorizationModel();
            try
            {
                result = new AuthorizationRepository().QueryANode(functionName);
                return new ApiResult<AuthorizationModel>(result);
            }
            catch (Exception ex)
            {
                return new ApiError<AuthorizationModel>("Exception", ex.Message);
            }
        }

        /// <summary>
        /// 新增/修改Authorization Setting
        /// </summary>
        /// <param name="insertData"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public ApiResult<bool> Update(AuthorizationDataModel insertData)
        {
            ApiResult<bool> apiResult = new ApiResult<bool>();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    OraDataService ora = new OraDataService();
                    #region MGT_AUTHORIZATION_SETTING
                    MGT_AUTHORIZATION_SETTING dbModel = new MGT_AUTHORIZATION_SETTING
                    {
                        FUNCTION_NAME = insertData.name,
                        DESCRIPTION = insertData.description,
                        LAST_UPDATED_TIMESTAMP = DateTime.Now,
                        LAST_UPDATED_USERID = insertData.lastupdateduserid,
                        FK_MGAU_ID = insertData.fkmgauid.ToInt64(),
                        LEVELS = insertData.levels,
                        AAMS_SYS_CODE = insertData.aams_syscode,
                        MENU_IND = insertData.menu
                    };

                    string action = "";
                    if (insertData.id == "0")
                    {
                        dbModel.ID = ora.GetSeqNo("MGAU_SEQ");
                        action = "Insert";
                        apiResult.Succ = ora.Insert(dbModel);
                    }
                    else
                    {
                        dbModel.ID = insertData.id.ToInt64();
                        action = "Update";
                        apiResult.Succ = ora.Update(dbModel, new
                        {
                            ID = insertData.id.ToInt64(),
                        }) == 1;
                    }

                    if (apiResult.Succ == false)
                    {
                        apiResult = new ApiError<bool>("500", $"{action} MGT_AUTHORIZATION_SETTING failed.");
                        return apiResult;
                    }

                    History(dbModel, action);
                    #endregion

                    #region MGT_AUTHORIZATION_TO
                    insertData.newauthorizedto = insertData.newauthorizedto.Where(x => string.IsNullOrEmpty(x.user_role) == false).ToList();
                    ModelComparer<AuthorizationToDataModel> compares = ModelComparer.Create(insertData.authorizedto,
                                                                                            insertData.newauthorizedto,
                                                                                            m => m.id);
                    #region Update
                    if (compares.Update.Count() > 0)
                    {
                        foreach (AuthorizationToDataModel x in compares.Update)
                        {
                            OraDataService ora_authto = new OraDataService();

                            MGT_AUTHORIZATION_TO dbModel_To = new MGT_AUTHORIZATION_TO
                            {
                                ID = x.id,
                                USER_ROLE = x.user_role,
                                LAST_UPDATED_TIMESTAMP = DateTime.Now,
                                LAST_UPDATED_USERID = x.lastupdateduserid,
                                FK_MGAU_ID = dbModel.ID,
                                EFFECTIVE_START = x.effectivestart.ToDateTime(),
                                EFFECTIVE_END = x.effectiveend.ToDateTime()
                            };

                            apiResult.Succ = ora_authto.Update(dbModel_To, new
                            {
                                ID = insertData.id,
                            }) == 1;

                            if (apiResult.Succ == false)
                            {
                                apiResult = new ApiError<bool>("500", "Update MGT_AUTHORIZATION_TO failed.");
                                return apiResult;
                            }

                            HistoryTo(dbModel_To, "Update");
                        }
                    }
                    #endregion

                    #region Insert
                    if (compares.Insert.Count() > 0)
                    {
                        foreach (AuthorizationToDataModel x in compares.Insert)
                        {
                            OraDataService ora_authto = new OraDataService();
                            MGT_AUTHORIZATION_TO dbModel_To = new MGT_AUTHORIZATION_TO
                            {
                                ID = ora_authto.GetSeqNo("MGAU_SEQ"),
                                USER_ROLE = x.user_role,
                                LAST_UPDATED_TIMESTAMP = DateTime.Now,
                                LAST_UPDATED_USERID = x.lastupdateduserid,
                                FK_MGAU_ID = dbModel.ID,
                                EFFECTIVE_START = x.effectivestart.ToDateTime(),
                                EFFECTIVE_END = x.effectiveend.ToDateTime()
                            };

                            apiResult.Succ = ora_authto.Insert(dbModel_To);
                            if (apiResult.Succ == false)
                            {
                                apiResult = new ApiError<bool>("500", "Insert MGT_AUTHORIZATION_TO failed.");
                                return apiResult;
                            }

                            HistoryTo(dbModel_To, "Insert");
                        }
                    }
                    #endregion

                    #region Delete
                    if (compares.Delete.Count() > 0)
                    {
                        foreach (AuthorizationToDataModel x in compares.Delete)
                        {
                            OraDataService ora_authto = new OraDataService();
                            apiResult.Succ = ora_authto.Delete<MGT_AUTHORIZATION_TO>(new
                            {
                                ID = x.id
                            }) == 1;

                            if (apiResult.Succ == false)
                            {
                                apiResult = new ApiError<bool>("500", "Delete MGT_AUTHORIZATION_TO failed.");
                                return apiResult;
                            }

                            HistoryTo(new MGT_AUTHORIZATION_TO
                            {
                                LAST_UPDATED_USERID = insertData.lastupdateduserid
                            }, "Delete");
                        }
                    }
                    #endregion

                    #endregion
                    ts.Complete();
                    apiResult.Message = "Update Success.";
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
        /// 刪除Authorization Setting
        /// </summary>
        /// <param name="insertData"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public ApiResult<bool> Delete(AuthorizationDataModel insertData)
        {
            ApiResult<bool> apiResult = new ApiResult<bool>();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    OraDataService ora = new OraDataService();

                    apiResult.Succ = ora.Delete<MGT_AUTHORIZATION_SETTING>(new
                    {
                        ID = insertData.id.ToInt64()
                    }) == 1;

                    if (apiResult.Succ == false)
                    {
                        apiResult = new ApiError<bool>("500", "Delete MGT_AUTHORIZATION_SETTING failed.");
                        return apiResult;
                    }

                    History(new MGT_AUTHORIZATION_SETTING
                    {
                        FUNCTION_NAME = insertData.name,
                        DESCRIPTION = insertData.description,
                        LEVELS = insertData.levels,
                        LAST_UPDATED_USERID = insertData.lastupdateduserid
                    }, "Delete");

                    ts.Complete();
                    apiResult.Message = "Delete Success.";
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                apiResult = new ApiError<bool>("Exception", ex.Message);
            }
            return apiResult;
        }

        private List<AuthorizationDataModel> AuthSettingTreeBuild(List<MGT_AUTHORIZATION_SETTING> db_auth_setting,
                                                                 List<MGT_AUTHORIZATION_TO> db_auth_to,
                                                                 List<AuthorizationSettingHistoryDataModel> dataModel_ashd = null,
                                                                 List<AuthorizationToHistoryDataModel> dataModel_mgth = null,
                                                                 bool selectAll = false)
        {
            db_auth_setting = db_auth_setting ?? new List<MGT_AUTHORIZATION_SETTING>();
            db_auth_to = db_auth_to ?? new List<MGT_AUTHORIZATION_TO>();
            dataModel_ashd = dataModel_ashd ?? new List<AuthorizationSettingHistoryDataModel>();
            dataModel_mgth = dataModel_mgth ?? new List<AuthorizationToHistoryDataModel>();

            List<AuthorizationDataModel> result = new List<AuthorizationDataModel>();

            List<AuthorizationToDataModel> dataModel_atd = (from x in db_auth_to
                                                            orderby x.ID
                                                            select new AuthorizationToDataModel
                                                            {
                                                                id = x.ID,
                                                                lastupdatedtimestamp = x.LAST_UPDATED_TIMESTAMP,
                                                                lastupdateduserid = x.LAST_UPDATED_USERID,
                                                                fkmgauid = x.FK_MGAU_ID,
                                                                effectivestart = x.EFFECTIVE_START.ToString("yyyy-MM-dd"),
                                                                effectiveend = x.EFFECTIVE_END.ToString("yyyy-MM-dd"),
                                                                user_role = x.USER_ROLE,
                                                                state = Extension.DateIntervalDuplicateCheck(x.EFFECTIVE_START, x.EFFECTIVE_END, DateTime.Now, null)
                                                            }).ToList();
            List<long> authSetting = dataModel_atd.Select(x => x.fkmgauid).ToList();

            for (int i = db_auth_setting.Max(j => j.LEVELS); i >= 0; i--)
            {
                List<AuthorizationDataModel> dataModelx = new List<AuthorizationDataModel>();
                dataModelx = (from x in db_auth_setting
                              orderby x.ID
                              let _authTo = dataModel_atd.Where(y => y.fkmgauid == x.ID).ToList()
                              let _children = result.Where(y => y.fkmgauid == x.ID.ToSafeString()).ToList()
                              where selectAll ? x.LEVELS == i : (x.LEVELS == i) && (_authTo.HasValue() || _children.HasValue())
                              select new AuthorizationDataModel
                              {
                                  id = x.ID.ToSafeString(),
                                  name = x.FUNCTION_NAME,
                                  aams_syscode = x.AAMS_SYS_CODE,
                                  description = x.DESCRIPTION,
                                  lastupdatedtimestamp = x.LAST_UPDATED_TIMESTAMP,
                                  lastupdateduserid = x.LAST_UPDATED_USERID,
                                  fkmgauid = x.FK_MGAU_ID.ToSafeString(),
                                  levels = x.LEVELS,
                                  menu = x.MENU_IND,
                                  children = _children,
                                  authorizedto = _authTo,
                                  newauthorizedto = _authTo,
                                  authorsethistory = dataModel_ashd.Where(y => y.fkmgauid == x.ID).ToList(),
                                  //auth_to_history = dataModel_mgth.Where(y => y.fk_mgat_seq)
                              }).ToList();
                authSetting.AddRange(dataModelx.Where(y => y.fkmgauid.ToInt64() > 0).Select(y => y.fkmgauid.ToInt64()));
                result = dataModelx;
            }

            return result;
        }

        private bool History(MGT_AUTHORIZATION_SETTING insertData, String action)
        {
            bool result = false;

            OraDataService ora = new OraDataService();
            MGT_AUTHORIZATION_SETTING_HISTORY dbModel = new MGT_AUTHORIZATION_SETTING_HISTORY
            {
                ID = ora.GetSeqNo("MGUH_SEQ"),
                FUNCTION_NAME = insertData.FUNCTION_NAME,
                DESCRIPTION = insertData.DESCRIPTION,
                LEVELS = insertData.LEVELS,
                AAMS_SYS_CODE = insertData.AAMS_SYS_CODE,
                ACTION = action,
                LAST_UPDATED_TIMESTAMP = DateTime.Now,
                LAST_UPDATED_USERID = insertData.LAST_UPDATED_USERID,
                FK_MGAU_ID = insertData.ID
            };

            result = ora.Insert(dbModel);
            return result;
        }

        private bool HistoryTo(MGT_AUTHORIZATION_TO newModel, String action)
        {
            bool result = false;

            OraDataService ora = new OraDataService();

            MGT_AUTHORIZATION_TO_HISTORY dbModel = new MGT_AUTHORIZATION_TO_HISTORY
            {
                ID = ora.GetSeqNo("MGTH_SEQ"),
                USER_ROLE = newModel.USER_ROLE,
                ACTION = action,
                EFFECTIVE_START = newModel.EFFECTIVE_START,
                EFFECTIVE_END = newModel.EFFECTIVE_END,
                LAST_UPDATED_TIMESTAMP = DateTime.Now,
                LAST_UPDATED_USERID = newModel.LAST_UPDATED_USERID,
                FK_MGAU_ID = newModel.FK_MGAU_ID,
            };
            result = ora.Insert(dbModel);

            return result;
        }
    }
}