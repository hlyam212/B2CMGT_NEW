using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVABMS_AP.Interface
{
    public class AAMSAutoAuthInputModel
    {
        private string _UserList = null;

        /// <summary>
        /// 應用系統代碼，例FFAMS0001
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 課別，例P38
        /// </summary>
        public string UnitCode { get; set; }
        /// <summary>
        /// 權限申請類型
        /// 1.	新增= A / 異動=U / 刪除=D / 組織異動=C(批次主動發起) / E=權限到期自動刪除 / F=預設權限異動單
        /// 2.	新增時：須帶AddPermissionList
        /// 3.	異動時：AddPermissionList與DelPermissionList不能皆為空白
        /// </summary>
        public string ApplyType { get; set; }
        /// <summary>
        /// 身分類別
        /// U = 使用人 UserList必填, DepartmentList為空
        /// D = 部門, 則DepartmentList 必填, UserList為空
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 申請單編號，例 INVXXX20210104001
        /// </summary>
        public string ApplyNo { get; set; }
        /// <summary>
        /// 申請單位，例P38
        /// </summary>
        public string ApplyUnit { get; set; }
        /// <summary>
        /// 授權到期日, 例2022/01/04
        /// ApplyType=E時, 必填
        /// </summary>
        public DateTime PerEndTime { get; set; }
        /// <summary>
        /// 申請單內容說明
        /// </summary>
        public string ApplyDesc { get; set; }
        /// <summary>
        /// 申請時間，例2021/01/04 14:10
        /// </summary>
        public DateTime ApplyTime { get; set; }
        /// <summary>
        /// 新增申請權限清單
        /// </summary>
        public List<Permission>? AddPermissionList { get; set; }
        /// <summary>
        /// 刪除權限清單
        /// </summary>
        public List<Permission>? DelPermissionList { get; set; }
        /// <summary>
        /// 使用人清單(加密字串)
        /// </summary>
        public string UserList
        {
            get { return _UserList; }
            set
            {
                _UserList = value;
                if (string.IsNullOrWhiteSpace(value) == false)
                {

                }
            }
        }
        /// <summary>
        /// 使用人清單(解密內容)
        /// </summary>
        public List<User>? UserListDecrypt { get; set; }
        /// <summary>
        /// 部門清單
        /// </summary>
        public List<Department>? DepartmentList { get; set; }

        public class Permission
        {
            /// <summary>
            /// 權限類別
            /// R=代表角色; F=功能程式; P=參數;
            /// </summary>
            public string Kind { get; set; }
            /// <summary>
            /// 父層代碼
            /// </summary>
            public string ParentCode { get; set; }
            /// <summary>
            /// 權限代碼
            /// </summary>
            public string Code { get; set; }
        }

        public class User
        {
            public string OrgPK { get; set; }
            public string DepartmentCode { get; set; }
            public string DepartmentName { get; set; }
            public string Title { get; set; }
            public string CName { get; set; }
            public string EName { get; set; }
            public string CompName { get; set; }
            public string AD { get; set; }
            public string SysAcc { get; set; }
            public string Email { get; set; }
            public string Tel { get; set; }
        }

        public class Department 
        {
            public string DomainName { get; set; }
            public string DepartmentCode { get; set; }
        }
    }
}
