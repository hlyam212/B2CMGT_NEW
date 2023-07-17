namespace EVABMS_AP.Model
{
    public class MGT_USERS
    {
        public long ID { get; set; }
        public string? DEPARTMENT { get; set; }
        public string? USERID { get; set; }
        public string AAMS_SYS_CODE { get; set; }
        public DateTime LAST_UPDATED_TIMESTAMP  { get; set; }
        public string LAST_UPDATED_USERID { get; set; }
        public string? USER_CNAME { get; set; }
        public string? USER_ENAME { get; set; }
        public string? EMAIL { get; set; }
        public string AAMS_CATEGORY { get; set; }
        public DateTime? LAST_LOGIN_TIMESTAMP { get; set; }

    }
}
