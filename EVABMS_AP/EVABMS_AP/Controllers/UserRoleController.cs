using CommonHelper;
using EVABMS.AP.Authorization.Domain.Entities;
using EVABMS.AP.Authorization.Infrastructure;
using EVABMS_AP.Interface;
using EVABMS_AP.Model;
using Microsoft.AspNetCore.Mvc;
using OracleHelper.TransactSql;
using System.Collections.Generic;
using UtilityHelper;

namespace EVABMS_AP.Controllers
{
    /// <summary>
    /// User Role相關的WEB API
    /// </summary>
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class UserRoleController : ControllerBase
    {
        /// <summary>
        /// 找出全部的UserRole
        /// </summary>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpGet]
        [Route("Roles")]
        public ApiResult<List<string>> QueryAllRoles()
        {
            ApiResult<List<string>> result = new ApiResult<List<string>>();

            try
            {
                OracleService ora = new OracleService();
                List<UserRole> data = new UserRoleRepository().QueryAllRoles();

                result = new ApiResult<List<string>>(data.Select(x => x.Role).ToList());
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<List<string>>("Exception", ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 找出全部的UserRole
        /// </summary>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        [HttpGet]
        public ApiResult<List<UserRoleDataModel>> Query()
        {
            List<UserRoleDataModel> resultList = new();
            try
            {
                OraDataService ora = new();
                string sql = @" SELECT * FROM MGT_USER_ROLES";
                List<MGT_USER_ROLESDataModel> mgtuserroles = ora.Select<MGT_USER_ROLESDataModel>(sql);

                sql = @" SELECT DISTINCT USER_ROLE FROM MGT_AUTHORIZATION_TO";
                List<string> mgtauthorizationto = ora.Select<MGT_USER_ROLES>(sql).Select(x => x.USER_ROLE).ToList();

                sql = @" SELECT DISTINCT USER_ROLE FROM MGT_AUTHORIZED_USERS";
                List<string> mgtauthorizedusers = ora.Select<MGT_USER_ROLES>(sql).Select(x => x.USER_ROLE).ToList();

                foreach (MGT_USER_ROLESDataModel x in mgtuserroles)
                {
                    UserRoleDataModel userRoleDataModel = new();

                    //userrole
                    x.editable = true;
                    if (mgtauthorizationto.Contains(x.user_role) || mgtauthorizedusers.Contains(x.user_role))
                    {
                        x.editable = false;
                    }
                    userRoleDataModel.Userrole = x;

                    //rolememberlist
                    ora = new();
                    sql = @"SELECT MGT_USERS.DEPARTMENT,
                                   MGT_USERS.USERID,
                                   MGT_USERS.AAMS_SYS_CODE,
                                   MGT_USERS.USER_CNAME,
                                   MGT_USERS.USER_ENAME,
                                   MGT_USERS.EMAIL,
                                   MGT_USERS.LAST_LOGIN_TIMESTAMP
                            FROM   MGT_USERS,MGT_AUTHORIZED_USERS
                            WHERE  MGT_USERS.ID=MGT_AUTHORIZED_USERS.FK_MGUS_SEQ
                            AND    MGT_AUTHORIZED_USERS.USER_ROLE = :USER_ROLE";

                    ora.SetOraParameters("USER_ROLE", $"{x.user_role}", x.user_role.OraType());
                    List<MGT_USERSDataModel> mgtusers = ora.Select<MGT_USERSDataModel>(sql);
                    userRoleDataModel.Rolememberlist = mgtusers;

                    //rolefunctionlist
                    AuthorizationController authorizationController = new();
                    List<string> roles = new() { x.user_role };
                    ApiResult<List<AuthorizationDataModel>> authresult = authorizationController.Query(roles);
                    userRoleDataModel.Rolefunctionlist = authresult.Data;

                    resultList.Add(userRoleDataModel);
                }

                return new ApiResult<List<UserRoleDataModel>>(resultList);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return new ApiError<List<UserRoleDataModel>>("Exception", ex.Message);
            }
        }

        /// <summary>
        /// 更新DB
        /// </summary>
        /// <param name="newmodel"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public ApiResult<bool> Update(List<MGT_USER_ROLESDataModel> newmodel)
        {
            ApiResult<bool> result = new();
            try
            {
                OraDataService ora = new();
                string sql = @" SELECT * FROM MGT_USER_ROLES";
                List<MGT_USER_ROLESDataModel> oldmodel = ora.Select<MGT_USER_ROLESDataModel>(sql);
                ModelComparer<MGT_USER_ROLESDataModel> compares = ModelComparer.Create(oldmodel, newmodel, m => m.user_role);
                List<MGT_USER_ROLESDataModel> delete = compares.Delete.ToList();
                List<MGT_USER_ROLESDataModel> insert = compares.Insert.ToList();
                List<MGT_USER_ROLESDataModel> update = compares.Update.ToList();
                if (!delete.IsNullOrEmpty())
                {
                    foreach (MGT_USER_ROLESDataModel x in delete)
                    {
                        OraDataService deleteora = new();
                        result.Succ = deleteora.Delete<MGT_USER_ROLESDataModel>(new
                        {
                            USER_ROLE = x.user_role,
                        }) == 1;
                    }
                }
                if (!insert.IsNullOrEmpty())
                {
                    foreach (MGT_USER_ROLESDataModel x in insert)
                    {
                        OraDataService insertora = new();
                        x.last_updated_timestamp = DateTime.Now;
                        result.Succ = insertora.Insert(x);
                        if (!result.Succ) return result = new ApiError<bool>(null, "Insert failed.");
                    }
                }
                if (!update.IsNullOrEmpty())
                {
                    foreach (MGT_USER_ROLESDataModel x in update)
                    {
                        OraDataService updateora = new();
                        x.last_updated_timestamp = DateTime.Now;
                        result.Succ = updateora.Update(x, new { USER_ROLE = x.user_role }) == 1;
                        if (!result.Succ) return result = new ApiError<bool>(null, "Update failed.");
                    }
                }
                result.Succ = true;
                result.Message = $"UPDATED {compares.Update.Count()} DATA; DELETED {compares.Delete.Count()} DATA; INSERT {compares.Insert.Count()} DATA";
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                result = new ApiError<bool>("Exception", ex.Message);
            }
            return result;
        }
    }
}
