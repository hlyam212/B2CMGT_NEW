using EVABMS.AP.Authorization.Domain.Entities;
using OracleHelper.TransactSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UtilityHelper;

namespace EVABMS.AP.Authorization.Infrastructure
{
    public class UserRepository
    {
        /// <summary>
        /// 依照USERID找出有被授權的User Role
        /// </summary>
        /// <param userid="H39232">User ID</param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        public User Query(string userid)
        {
            OracleService ora = new OracleService();
            User quitirua = new User().SetAD(userid);
            User result = ora.SelectByPrimaryKey<User>(quitirua).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// 依照USERID找出有被授權的User Role
        /// </summary>
        /// <param userid="H39232">User ID</param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        public List<string> QueryUserRole(string userid)
        {
            List<string> result = new List<string>();

            OracleService ora = new OracleService();

            #region Query MGT_AUTHORIZED_USERS
            string sql = @"  SELECT
                                    DISTINCT(ROLES.USER_ROLE) user_role
                             FROM
                                    MGT_USERS USERS
                             JOIN   MGT_AUTHORIZED_USERS ROLES ON USERS.USERID = ROLES.USERID
                             WHERE  USERS.USERID=:USERID";
            ora.SetOraParameters("USERID", userid, OraDataType.Varchar2);
            result = ora.Select<AuthTo>(sql).Select(x => x.userrole).ToList();
            #endregion

            return result;
        }

        /// <summary>
        /// 傳入UserId & Function Name, 判斷這個User有無權限使用
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public bool IsAuthToFunction(string userid, string functionName)
        {
            bool result = false;

            List<string> userrole = QueryUserRole(userid);
            if (userrole == null)
            {
                return false;
            }
            if (userrole.Contains("OMNISCIENT"))
            {
                return true;
            }
            AuthorizationModel funcNode = new AuthorizationRepository().QueryANode(functionName);
            if (funcNode == null)
            {
                return false;
            }

            result = funcNode.authto.Where(x => userrole.Contains(x.userrole)).Count() > 0;
            return result;
        }

        public int Update(User target, User org)
        {
            OracleService oraService = new OracleService();
            int result = oraService.UpdateByPrimaryKey(updateMoodel: target, originalModel: org);
            return result;
        }
    }
}
