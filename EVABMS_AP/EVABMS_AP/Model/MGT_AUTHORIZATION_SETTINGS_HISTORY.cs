namespace EVABMS_AP.Model
{
    public class MGT_AUTHORIZATION_SETTING_HISTORY
    {
        public long ID { get; set; }
        public string FUNCTION_NAME { get; set; }
        public string? DESCRIPTION { get; set; }
        public long LEVELS { get; set; }
        public string? AAMS_SYS_CODE { get; set; }
        public string? ACTION { get; set; }
        public DateTime LAST_UPDATED_TIMESTAMP  { get; set; }
        public string LAST_UPDATED_USERID { get; set; }
        public long? FK_MGAU_ID { get; set; }
    }
}
