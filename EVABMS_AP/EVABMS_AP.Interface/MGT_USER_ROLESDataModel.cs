using System;
namespace EVABMS_AP.Interface
{
    [DbTable("MGT_USER_ROLES")]
    public class MGT_USER_ROLESDataModel
    {
        [DbColumn("USER_ROLE")]
        public string? user_role { get; set; }
        [DbColumn("DESCRIPTION")]
        public string? description { get; set; }
        [DbColumn("LAST_UPDATED_TIMESTAMP")]
        public DateTime? last_updated_timestamp { get; set; }
        [DbColumn("LAST_UPDATED_USERID")]
        public string? last_updated_userid { get; set; }
        public bool? editable { get; set; }
    }
}
