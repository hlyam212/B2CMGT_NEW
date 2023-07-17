using EVABMS_AP.Interface;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using CommonHelper;
using Newtonsoft.Json;
using UtilityHelper;

namespace B2CMGT_NEW.Test
{
    [TestClass]
    public class AAMSTestIntegrated
    {
        [TestMethod]
        public async Task AAMS_�s�W�v��()
        {
            #region �����
            AAMSAutoAuthInputModel input = new AAMSAutoAuthInputModel
            {
                SysCode = "FFAMS0001",
                UnitCode = "P13",
                ApplyType = "A",
                Category = "U",
                ApplyNo = "INVXXX20210104001",
                ApplyUnit = "P13",
                PerEndTime = DateTime.MinValue,
                ApplyDesc = "UnitTest AAMS_�s�W�v��",
                ApplyTime = DateTime.Now,
                AddPermissionList = new List<AAMSAutoAuthInputModel.Permission> {
                    new AAMSAutoAuthInputModel.Permission
                    {
                        Kind="R",
                        Code="CPU"
                    }
                },
                UserListDecrypt = new List<AAMSAutoAuthInputModel.User>
                {
                    new AAMSAutoAuthInputModel.User{
                        DepartmentCode="CPU/SD1/P13",
                        DepartmentName="�q�⥻��",
                        Title="�Ƥu�{�v",
                        CName="�����",
                        EName="Kevin",
                        CompName="���a���",
                        AD="F12345",
                        Email="F12345@evaair.com",
                        Tel="351-1234"
                    },
                    new AAMSAutoAuthInputModel.User{
                        DepartmentCode="CPU/SD1/P13",
                        DepartmentName="�q�⥻��",
                        Title="�Ƥu�{�v",
                        CName="���@�_",
                        EName="Tina",
                        CompName="���a���",
                        AD="F12389",
                        Email="F12389@evaair.com",
                        Tel="351-1235"
                    },
                }
            };
            #endregion

            //string WBSResultJson = await new WebCommonHelper.Services.CallApi.CallAPI().Get("", "Authorization");

            var result = JsonConvert.SerializeObject(input);

            //var result = new AAMSController().AutoAuth(input);
        }
    }
}