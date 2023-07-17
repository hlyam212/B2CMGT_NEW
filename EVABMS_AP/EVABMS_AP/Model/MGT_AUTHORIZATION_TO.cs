namespace EVABMS_AP.Model
{
    public class MGT_AUTHORIZATION_TO
    {
        public long ID { get; set; }
        public string? USER_ROLE { get; set; }
        public DateTime LAST_UPDATED_TIMESTAMP { get; set; }
        public string? LAST_UPDATED_USERID { get; set; }
        public long FK_MGAU_ID { get; set; }
        public DateTime EFFECTIVE_START { get; set; }
        public DateTime EFFECTIVE_END { get; set; }
    }
}
