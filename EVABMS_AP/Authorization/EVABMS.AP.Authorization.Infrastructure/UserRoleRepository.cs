using EVABMS.AP.Authorization.Domain.Entities;
using OracleHelper.TransactSql;

namespace EVABMS.AP.Authorization.Infrastructure
{
    public class UserRoleRepository
    {
        public List<UserRole> QueryAllRoles() 
        {
            OracleService ora = new OracleService();
            List<UserRole> data = ora.Select<UserRole>();
            return data;
        }

        /// <summary>
        /// 找出全部的UserRole
        /// </summary>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        public List<UserRoleModel> Query()
        {
            List<UserRoleModel> result = new List<UserRoleModel>();
            OracleService ora = new OracleService();
            List<UserRole> userRole = ora.Select<UserRole>();
            List<AuthTo> authTo = ora.Select<AuthTo>();
            List<UserAuthorized> userAuthorized = ora.Select<UserAuthorized>();

            foreach (UserRole role in userRole)
            {
                if (authTo.Exists(x => x.userrole == role.Role) ||
                    userAuthorized.Exists(x => x.UserRole == role.Role))
                {
                    role.SetEditable(false);
                }
                else
                {
                    role.SetEditable(true);
                }

            }

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
                                JOIN    MGT_AUTHORIZED_USERS ROLES ON USERS.ID = ROLES.FK_MGUS_SEQ
                                WHERE   USERS.USERID=:USERID";
            ora.SetOraParameters("USERID", userid, OraDataType.Varchar2);
            result = ora.Select<AuthTo>(sql).Select(x => x.userrole).ToList();
            #endregion

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
