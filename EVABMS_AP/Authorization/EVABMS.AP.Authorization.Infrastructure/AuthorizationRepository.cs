using EVABMS.AP.Authorization.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using OracleHelper.TransactSql;
using System.Runtime.InteropServices;
using UtilityHelper;

namespace EVABMS.AP.Authorization.Infrastructure
{
    public class AuthorizationRepository
    {
        List<AuthSetting> authSetting = new List<AuthSetting>();
        List<AuthTo> authTo = new List<AuthTo>();
        List<AuthSettingHistory> settingHistory = new List<AuthSettingHistory>();

        public AuthorizationRepository(bool preSetQuery = false)
        {
            if (preSetQuery)
            {
                OracleService ora = new OracleService();
                authSetting = ora.Select<AuthSetting>();
                authTo = ora.Select<AuthTo>();
                settingHistory = ora.Select<AuthSettingHistory>();
            }
        }

        /// <summary>
        /// 找出全部的Authorization Setting
        /// </summary>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        public List<AuthorizationModel> Query()
        {
            OracleService ora = new OracleService();

            if (authSetting.IsNullOrEmpty()) authSetting = ora.Select<AuthSetting>();
            if (authTo.IsNullOrEmpty()) authTo = ora.Select<AuthTo>();
            if (settingHistory.IsNullOrEmpty()) settingHistory = ora.Select<AuthSettingHistory>();

            List<AuthorizationModel> result = BuildTree(selectAll: true);

            return result;
        }

        /// <summary>
        /// 依照USERID找出有被授權的Authorization Setting
        /// </summary>
        /// <param userid="H39232">User ID</param>
        /// <returns></returns>
        /// <remarks>這裡是註解</remarks>
        public List<AuthorizationModel> Query(List<string> userRoles,bool menuIND=false)
        {
            OracleService ora = new OracleService();

            if (authSetting.IsNullOrEmpty()) authSetting = ora.Select<AuthSetting>();
            if (authTo.IsNullOrEmpty()) authTo = ora.Select<AuthTo>();
            //只找出生效的
            authTo = authTo.Where(x => userRoles.Contains(x.userrole) &&
                                       Extension.DateIntervalDuplicateCheck(x.effectivedtstart, x.effectivedtend, DateTime.Now, null)).ToList();

            List<AuthorizationModel> result = BuildTree(menuIND: menuIND);

            return result;
        }

        /// <summary>
        /// 傳入Function Name, 找出這一個和下一層的Authorization Setting 和 Authorization To
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public AuthorizationModel? QueryANode(string auth)
        {
            OracleService ora = new OracleService();

            #region Query MGT_AUTHORIZATION_SETTING
            if (authSetting.IsNullOrEmpty()) authSetting = ora.Select<AuthSetting>();
            AuthSetting thisFunc = authSetting.FirstOrDefault(x => x.auth == auth);
            if (thisFunc == null)
            {
                return null;
            }
            #endregion

            #region Query MGT_AUTHORIZATION_TO
            if (authTo.IsNullOrEmpty()) authTo = ora.Select<AuthTo>();

            //只找出生效的
            authTo = authTo.Where(x => Extension.DateIntervalDuplicateCheck(x.effectivedtstart, x.effectivedtend, DateTime.Now, null)).ToList();
            #endregion

            AuthorizationModel result = new AuthorizationModel(thisFunc,
                                                               ((from x in authSetting
                                                                 where x.fk_mgau_id == thisFunc.id
                                                                 select new AuthorizationModel(x,
                                                                                               null,
                                                                                               authTo.Where(y => y.fk_mgau_id == x.id).ToList(),
                                                                                               null)).ToList()),
                                                               (authTo.Where(y => y.fk_mgau_id == thisFunc.id).ToList()),
                                                               null);
            result.children.ForEach(x => x.SetAuthTo(authTo.Where(y => y.fk_mgau_id == x.setting.id).ToList()));

            return result;
        }

        private List<AuthorizationModel> BuildTree(bool selectAll = false, bool menuIND = false)
        {
            List<AuthorizationModel> result = new List<AuthorizationModel>();

            authTo.ForEach(x => x.SetStatus(DateTime.Now, null));
            List<long> authSettingIDs = authTo.Select(x => x.fk_mgau_id).ToList();

            for (int i = authSetting.Max(j => j.levels); i >= 0; i--)
            {
                List<AuthorizationModel> temp = (from x in authSetting
                                                 orderby x.id
                                                 let _authTo = authTo.Where(y => y.fk_mgau_id == x.id).ToList()
                                                 let _children = result.Where(y => y.setting.fk_mgau_id == x.id).ToList()
                                                 let _history = settingHistory.Where(y => y.fk_mgau_id == x.id).ToList()
                                                 let _menu = menuIND ? x.menu == "Y" : true
                                                 where selectAll ? x.levels == i : (x.levels == i) && (_authTo.HasValue() || _children.HasValue()) && _menu
                                                 select AuthorizationModel.Create(x, _children, _authTo, _history)).ToList();
                authSettingIDs.AddRange(temp.Where(y => y.setting.fk_mgau_id > 0).Select(y => y.setting.fk_mgau_id));
                result = temp;
            }

            return result;
        }
    }
}