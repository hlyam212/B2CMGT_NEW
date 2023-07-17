using System.ComponentModel.DataAnnotations.Schema;

namespace EVABMS_AP.Interface
{
    [DbTable("MGT_PARAMETER_SETTINGS")]
    public class ParameterSettingDataModel
    {
        [DbColumn("ID")]
        public long id { get; set; }
        [DbColumn("FUNCTION_NAME")]
        public string? functionname { get; set; }
        [DbColumn("SUB_FUNCTION_NAME")]
        public string? subfunctionname { get; set; }
        [DbColumn("SETTING_NAME")]
        public string? settingname { get; set; }
        [DbColumn("VALUE")]
        public string? value { get; set; }
        [DbColumn("DESCRIPTION")]
        public string? description { get; set; }
        [DbColumn("LAST_UPDATED_TIMESTAMP")]
        public DateTime lastupdatedtimestamp { get; set; }
        [DbColumn("LAST_UPDATED_USERID")]
        public string? lastupdateduserid { get; set; }
        [DbColumn("FK_MGAU_ID")]
        public long fk_mgau_id { get; set; }
        public string? Auth { get; set; }
    }
}
