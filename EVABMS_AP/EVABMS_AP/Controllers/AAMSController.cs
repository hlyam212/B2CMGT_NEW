using Asp.Versioning;
using EVABMS_AP.Interface;
using EVABMS_AP.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OracleHelper.TransactSql;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using UtilityHelper;
using static EVABMS_AP.Interface.AAMSAutoAuthInputModel;


namespace EVABMS_AP.Controllers
{
    /// <summary>
    /// AAMS相關的WEB API
    /// </summary>
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class AAMSController : ControllerBase
    {
        /// <summary>
        /// 依照搜尋條件找出UserRelation
        /// </summary>
        /// <param name="unitcode"></param>
        /// <param name="syscode"></param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpPost("UserRelation")]
        public AAMSAuthUsersyncModel AAMSAuthUser(string unitcode, string syscode)
        {
            ApiResult<bool> result = new ApiResult<bool>();
            List<AAMS_API_LOG> aams_log = new List<AAMS_API_LOG>();
            List<string> parameter = new List<string>();

            List<AAMSAuthUsersyncModelx> dataModelx = new List<AAMSAuthUsersyncModelx>();

            #region Query 帳號+授權資料
            OracleService ora = new OracleService();

            string sql = @"SELECT
                            A.DEPARTMENT UnitCode
                            ,A.AAMS_SYS_CODE SysCode
                            ,A.USERID UserId
                            ,B.USER_ROLE UserRole
                            FROM MGT_USERS A
                            JOIN MGT_AUTHORIZED_USERS B ON A.ID=B.FK_MGUS_SEQ
                            ";
            if (unitcode.HasValue())
            {
                parameter.Add(" A.DEPARTMENT LIKE :DEPARTMENT ");
                ora.SetOraParameters("DEPARTMENT", $"%{unitcode}%", unitcode.OraType());
            }
            if (syscode.HasValue())
            {
                parameter.Add(" A.AAMS_SYS_CODE LIKE :AAMS_SYS_CODE ");
                ora.SetOraParameters("AAMS_SYS_CODE", $"%{syscode}%", syscode.OraType());
            }
            if (parameter.HasValue())
            {
                sql += $" WHERE {string.Join(" AND ", parameter)}";
            }
            dataModelx = ora.Select<AAMSAuthUsersyncModelx>(sql);
            #endregion

            #region 組合AAMSAuthUsersyncModel資料
            List<AAMSAuthUsersyncModel.USERLIST> userlist = (from x in dataModelx
                                                             select new AAMSAuthUsersyncModel.USERLIST
                                                             {
                                                                 ADAccount = x.UserId,
                                                                 Kind = "R",
                                                                 Code = x.UserRole,
                                                             }).ToList();

            AAMSAuthUsersyncModel dataModel = new AAMSAuthUsersyncModel
            {
                UnitCode = unitcode,
                SysCode = syscode,
                UserList = userlist,
            };
            #endregion

            #region 新增查詢紀錄
            List<string> inputdata = JsonConvert.SerializeObject(dataModel).Chunk(3000)
                                     .Select(x => new string(x))
                                     .ToList();

            if (dataModelx.Count > 0)
            {
                for (int i = 0; i < inputdata.Count; i++)
                {
                    OraDataService orahistory = new OraDataService();

                    AAMS_API_LOG dbModelHistory = new AAMS_API_LOG
                    {
                        ID = orahistory.GetSeqNo("AAML_SEQ"),
                        FUNCTION = MethodBase.GetCurrentMethod().Name,
                        INPUT_DATA = inputdata[i],
                        SUCC = "Y",
                        LAST_UPDATED_TIMESTAMP = DateTime.Now,
                    };

                    if (i > 0)
                    {
                        dbModelHistory.FK_AAML_SEQ = aams_log[0].ID;
                    }

                    result.Succ = orahistory.Insert(dbModelHistory);

                    aams_log.Add(dbModelHistory);

                }
            }
            #endregion

            return dataModel;
        }

        /// <summary>
        /// 依照搜尋條件找出UserRoleGet
        /// </summary>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpGet("UserRoleGet")]
        public AAMSAuthCodesyncModel UR()
        {
            ApiResult<bool> result = new ApiResult<bool>();
            List<AAMS_API_LOG> aams_log = new List<AAMS_API_LOG>();
            OracleService ora = new OracleService();

            #region Query UserRole資料
            string sql_MGT_USER_ROLES = @" SELECT * FROM MGT_USER_ROLES ORDER BY USER_ROLE";

            List<MGT_USER_ROLES> dbModel = ora.Select<MGT_USER_ROLES>(sql_MGT_USER_ROLES);
            #endregion

            #region 組合AAMSAuthCodesyncMode資料
            List<AAMSAuthCodesyncModel.CODELIST> codelist = (from x in dbModel
                                                             orderby x.USER_ROLE
                                                             select new AAMSAuthCodesyncModel.CODELIST
                                                             {
                                                                 Kind = "R",
                                                                 PCode = "",
                                                                 ParentCode = "",
                                                                 Code = x.USER_ROLE,
                                                                 Name = x.DESCRIPTION,
                                                             }).ToList();

            AAMSAuthCodesyncModel dataModel = new AAMSAuthCodesyncModel
            {
                UnitCode = "P13",
                SysCode = "FFAMS0001",
                CodeList = codelist
            };
            #endregion

            #region 新增查詢紀錄
            List<string> inputdata = JsonConvert.SerializeObject(dataModel).Chunk(3000)
                                     .Select(x => new string(x))
                                     .ToList();

            if (dbModel.Count > 0)
            {
                for (int i = 0; i < inputdata.Count; i++)
                {
                    OraDataService orahistory = new OraDataService();

                    AAMS_API_LOG dbModelHistory = new AAMS_API_LOG
                    {
                        ID = orahistory.GetSeqNo("AAML_SEQ"),
                        FUNCTION = MethodBase.GetCurrentMethod().Name,
                        INPUT_DATA = inputdata[i],
                        SUCC = "Y",
                        LAST_UPDATED_TIMESTAMP = DateTime.Now,
                    };

                    if (i > 0)
                    {
                        dbModelHistory.FK_AAML_SEQ = aams_log[0].ID;
                    }

                    result.Succ = ora.Insert(dbModelHistory);

                    aams_log.Add(dbModelHistory);
                }
            }
            #endregion

            return dataModel;
        }

        /// <summary>
        /// 依照搜尋條件找出AutoAuth
        /// </summary>
        /// <param userid="H39232">Search Criteria</param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpPost("AutoAuth")]
        public AAMSAuthResultModel AutoAuth(AAMSAutoAuthInputModel input)
        {
            ApiResult<bool> result = new ApiResult<bool>();
            ApiResult<long> result_aaml;
            List<AAMS_API_LOG> aams_log = new List<AAMS_API_LOG>();
            AAMSAuthResultModel aams_result = new AAMSAuthResultModel();

            List<string> inputdata = JsonConvert.SerializeObject(input).Chunk(3000)
                                                 .Select(x => new string(x))
                                                 .ToList();

            try
            {
                #region 新增查詢紀錄
                for (int i = 0; i < inputdata.Count; i++)
                {
                    OraDataService ora = new OraDataService();

                    AAMS_API_LOG dbModel = new AAMS_API_LOG
                    {
                        ID = ora.GetSeqNo("AAML_SEQ"),
                        FUNCTION = MethodBase.GetCurrentMethod().Name,
                        INPUT_DATA = inputdata[i],
                        SUCC = "N",
                        LAST_UPDATED_TIMESTAMP = DateTime.Now,
                    };

                    if (i > 0)
                    {
                        dbModel.FK_AAML_SEQ = aams_log[0].ID;
                    }

                    result.Succ = ora.Insert(dbModel);

                    if (result.Succ == false)
                    {
                        aams_result.ResultCode = "1";
                        aams_result.Msg = "failed";
                        return aams_result;
                    }
                    aams_log.Add(dbModel);
                }
                #endregion

                #region User
                if (input.Category == "U" && input.UserListDecrypt.HasValue())
                {
                    foreach (User aUser in input.UserListDecrypt)
                    {
                        MGT_USERS usersData = MGTUsersQuery(input.SysCode, aUser.AD, null).FirstOrDefault();
                        if (usersData == null)
                        {
                            usersData = new MGT_USERS
                            {
                                DEPARTMENT = aUser.DepartmentCode,
                                USERID = aUser.AD,
                                AAMS_SYS_CODE = input.SysCode,
                                USER_CNAME = aUser.CName,
                                USER_ENAME = aUser.EName,
                                EMAIL = aUser.Email,
                                AAMS_CATEGORY = input.Category
                            };
                        }
                        else
                        {
                            usersData.DEPARTMENT = aUser.DepartmentCode;
                            usersData.USER_CNAME = aUser.CName;
                            usersData.USER_ENAME = aUser.EName;
                            usersData.EMAIL = aUser.Email;
                        }

                        result = MGTCommon(usersData, input.AddPermissionList, input.DelPermissionList);
                        if (result.Succ == false)
                        {
                            aams_result.ResultCode = "1";
                            aams_result.Msg = "failed";
                            return aams_result;
                        }
                    }
                }
                #endregion
                #region department
                else if (input.Category == "D" && input.DepartmentList.HasValue())
                {
                    foreach (Department aDep in input.DepartmentList)
                    {
                        MGT_USERS depData = MGTUsersQuery(input.SysCode, null, aDep.DepartmentCode).FirstOrDefault();
                        if (depData == null)
                        {
                            depData = new MGT_USERS
                            {
                                DEPARTMENT = aDep.DepartmentCode,
                                AAMS_SYS_CODE = input.SysCode,
                                AAMS_CATEGORY = input.Category
                            };
                        }
                        else
                        {
                            depData.DEPARTMENT = aDep.DepartmentCode;
                        }

                        result = MGTCommon(depData, input.AddPermissionList, input.DelPermissionList);
                        if (result.Succ == false)
                        {
                            aams_result.ResultCode = "1";
                            aams_result.Msg = "failed";
                            return aams_result;
                        }
                    }
                }
                #endregion

                #region 修改AAMS_API_LOG SUCC欄位
                foreach (AAMS_API_LOG x in aams_log)
                {
                    OraDataService ora_update = new OraDataService();

                    x.SUCC = "Y";
                    result.Succ = ora_update.Update(x, new
                    {
                        x.ID,
                    }) == 1;

                    if (result.Succ == false)
                    {
                        aams_result.ResultCode = "1";
                        aams_result.Msg = "failed";
                        return aams_result;
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return aams_result;
        }

        private List<MGT_USERS> MGTUsersQuery(string AAMSSysCode = null, string AD = null, string DEP = null)
        {
            List<string> parameter = new List<string>();

            OracleService ora = new OracleService();

            string sql_MGT_USERS = @" SELECT * FROM MGT_USERS";

            if (string.IsNullOrWhiteSpace(AD) == false)
            {
                parameter.Add(" USERID = :USERID ");
                ora.SetOraParameters("USERID", $"{AD}", AD.OraType());
            }

            if (string.IsNullOrWhiteSpace(DEP) == false)
            {
                parameter.Add(" DEPARTMENT = :DEPARTMENT ");
                ora.SetOraParameters("DEPARTMENT", $"{DEP}", DEP.OraType());
            }

            if (string.IsNullOrWhiteSpace(AAMSSysCode) == false)
            {
                parameter.Add(" AAMS_SYS_CODE = :AAMS_SYS_CODE ");
                ora.SetOraParameters("AAMS_SYS_CODE", $"{AAMSSysCode}", AAMSSysCode.OraType());
            }

            if (parameter.HasValue())
            {
                sql_MGT_USERS += $" WHERE {string.Join(" AND ", parameter)}";
            }

            List<MGT_USERS> result = ora.Select<MGT_USERS>(sql_MGT_USERS);
            return result;
        }

        /// <summary>
        /// 依照搜尋條件找出MGTAuthUsersQuery
        /// </summary>
        /// <param name="FKMGUSSEQ"></param>
        /// <param name="USER_ROLE"></param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpPost("MGTAuthUsersQuery")]
        public List<MGT_AUTHORIZED_USERS> MGTAuthUsersQuery(long FKMGUSSEQ = 0, string USER_ROLE = null)
        {
            List<string> parameter = new List<string>();

            OracleService ora = new OracleService();

            string sql_MGT_AUTHORIZED_USERS = @" SELECT * FROM MGT_AUTHORIZED_USERS";

            if (FKMGUSSEQ != 0)
            {
                parameter.Add(" FK_MGUS_SEQ = :FK_MGUS_SEQ ");
                ora.SetOraParameters("FK_MGUS_SEQ", $"{FKMGUSSEQ}", FKMGUSSEQ.OraType());
            }

            if (string.IsNullOrWhiteSpace(USER_ROLE) == false)
            {
                parameter.Add(" USER_ROLE = :USER_ROLE ");
                ora.SetOraParameters("USER_ROLE", $"{USER_ROLE}", USER_ROLE.OraType());
            }

            if (parameter.HasValue())
            {
                sql_MGT_AUTHORIZED_USERS += $" WHERE {string.Join(" AND ", parameter)}";
            }

            List<MGT_AUTHORIZED_USERS> result = ora.Select<MGT_AUTHORIZED_USERS>(sql_MGT_AUTHORIZED_USERS);
            return result;
        }

        /// <summary>
        /// 新增MGTUsersInsert共用函式
        /// </summary>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpPost("MGTUsersInsert")]
        public ApiResult<long> MGTUsersInsert(MGT_USERS input)
        {
            ApiResult<long> result = new ApiResult<long>();
            if (input == null) return result;
            OraDataService ora = new OraDataService();

            input.ID = ora.GetSeqNo("MGUS_SEQ");
            input.LAST_UPDATED_TIMESTAMP = DateTime.Now;
            input.LAST_UPDATED_USERID = "AAMS";
            result.Succ = ora.Insert(input);

            if (result.Succ) result = new ApiResult<long>(input.ID);
            else result.Message = "Insert MGT_USERS failed";

            return result;
        }

        /// <summary>
        /// 更新MGTUsersUpdate共用函式
        /// </summary>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpPost("MGTUsersUpdate")]
        public ApiResult<long> MGTUsersUpdate(MGT_USERS input)
        {
            ApiResult<long> result = new ApiResult<long>();
            if (input == null) return result;
            OraDataService ora = new OraDataService();

            input.LAST_UPDATED_TIMESTAMP = DateTime.Now;
            input.LAST_UPDATED_USERID = "AAMS";
            result.Succ = ora.Update(input, new
            {
                input.ID,
            }) == 1;

            if (result.Succ) result = new ApiResult<long>(input.ID);
            else result.Message = "Update MGT_USERS failed";

            return result;
        }

        private ApiResult<bool> MGTUsersAddPermission(List<Permission> addPermission, long mgusseqid)
        {
            ApiResult<bool> result = new ApiResult<bool>();
            //AAMSAuthResultModel aams_result = new AAMSAuthResultModel();
            if (addPermission.IsNullOrEmpty())
            {
                result.Succ = true;
                return result;
            }

            foreach (Permission aPermission in addPermission)
            {
                MGT_AUTHORIZED_USERS usersAuthData = MGTAuthUsersQuery(mgusseqid, aPermission.Code).FirstOrDefault();

                if (usersAuthData != null)
                {
                    result.Succ = false;
                    return result;
                }
                #region Insert MGT_AUTHORIZED_USERS
                else
                {
                    OraDataService ora = new OraDataService();

                    MGT_AUTHORIZED_USERS dbModel = new MGT_AUTHORIZED_USERS
                    {
                        ID = ora.GetSeqNo("MGUU_SEQ"),
                        FK_MGUS_SEQ = mgusseqid,
                        USER_ROLE = aPermission.Code,
                        EFFECTIVE_START = DateTime.Now,
                        LAST_UPDATED_TIMESTAMP = DateTime.Now,
                        LAST_UPDATED_USERID = "AAMS",
                    };

                    result.Succ = ora.Insert(dbModel);

                    if (result.Succ == false)
                    {
                        return result;
                    }
                }
                #endregion
            }
            return result;
        }

        private ApiResult<bool> MGTUsersDelPermission(List<Permission> delPermission, long mgusseqid)
        {
            ApiResult<bool> result = new ApiResult<bool>();
            //AAMSAuthResultModel aams_result = new AAMSAuthResultModel();
            if (delPermission.IsNullOrEmpty())
            {
                result.Succ = true;
                return result;
            }
            foreach (Permission dpermission in delPermission)
            {
                MGT_AUTHORIZED_USERS usersAuthData = MGTAuthUsersQuery(mgusseqid, dpermission.Code).FirstOrDefault();
                if (usersAuthData == null)
                {
                    result.Succ = false;
                    return result;
                }
                #region Delete MGT_AUTHORIZED_USERS
                OraDataService ora = new OraDataService();
                bool result_authuers = false;

                result.Succ = ora.Delete<MGT_AUTHORIZED_USERS>(new
                {
                    usersAuthData.ID,
                }) == 1;

                if (result.Succ == false)
                {
                    return result;
                }
                #endregion
            }
            return result;
        }

        private ApiResult<bool> MGTCommon(MGT_USERS usersData, List<Permission>? AddPermissionList, List<Permission>? DelPermissionList)
        {
            AAMSAuthResultModel aams_result = new AAMSAuthResultModel();
            ApiResult<long> result_user = new ApiResult<long>();
            ApiResult<bool> result = new ApiResult<bool>();

            using (TransactionScope ts = new TransactionScope())
            {
                #region Insert MGT_USERS
                if (usersData.ID == 0)
                {
                    result_user = MGTUsersInsert(usersData);

                    if (result_user.Succ == false)
                    {
                        result.Message = "faild";
                        return result;
                    }
                }
                #endregion
                #region Update MGT_USERS
                else
                {
                    result_user = MGTUsersUpdate(usersData);

                    if (result_user.Succ == false)
                    {
                        result.Message = "faild";
                        return result;
                    }
                }
                #endregion

                #region 處理AddPermission
                result = MGTUsersAddPermission(AddPermissionList, result_user.Data);
                if (result.Succ == false)
                {
                    result.Message = "faild";
                    return result;
                }

                #endregion

                #region 處理DelPermission
                result = MGTUsersDelPermission(DelPermissionList, result_user.Data);
                if (result.Succ == false)
                {
                    result.Message = "faild";
                    return result;
                }
                #endregion

                ts.Complete();

            }
            return result;
        }

        private ApiResult<bool> MGTAuthUsersInsert(MGT_USERS input)
        {
            ApiResult<bool> apiResult;
            bool result = false;

            if (input == null) return new ApiResult<bool>(result);

            OraDataService ora = new OraDataService();

            input.ID = ora.GetSeqNo("MGUU_SEQ");


            try
            {


            }
            catch (Exception ex)
            {

            }
            apiResult = new ApiResult<bool>(result);
            return apiResult;
        }

        /// <summary>
        /// AAMS制定的加密
        /// </summary>
        /// <param name="pText"></param>
        /// <param name="pKey"></param>
        /// <returns></returns>
        private string Encrypt(string pText, string pKey)
        {
            string returnStr = string.Empty;
            string[] KeyPar = pKey.Split(',');
            if (KeyPar.Length != 2)
            {
                return returnStr;
            }

            byte[] TextBytes = Encoding.UTF8.GetBytes(pText);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(KeyPar[1]);
                aesAlg.IV = Encoding.UTF8.GetBytes(KeyPar[0]);
                ICryptoTransform ecryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                byte[] outputData = ecryptor.TransformFinalBlock(TextBytes, 0, TextBytes.Length);
                returnStr = Convert.ToBase64String(outputData);
            }
            return returnStr;
        }

        /// <summary>
        /// AAMS制定的解密
        /// </summary>
        /// <param name="pText"></param>
        /// <param name="pKey"></param>
        /// <returns></returns>
        private string Decrypt(string pText, string pKey)
        {
            string returnStr = string.Empty;
            string[] KeyPar = pKey.Split(',');
            if (KeyPar.Length != 2)
            {
                return returnStr;
            }

            byte[] TextBytes = Convert.FromBase64String(pText);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(KeyPar[1]);
                aesAlg.IV = Encoding.UTF8.GetBytes(KeyPar[0]);
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                byte[] outputData = decryptor.TransformFinalBlock(TextBytes, 0, TextBytes.Length);
                returnStr = Encoding.GetEncoding("utf-8").GetString(outputData);
            }

            return returnStr;
        }
    }
}
