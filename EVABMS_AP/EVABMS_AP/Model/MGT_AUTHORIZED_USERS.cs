namespace EVABMS_AP.Model
{
    public class MGT_AUTHORIZED_USERS
    {
        public long ID { get; set; }
        public long? FK_MGUS_SEQ { get; set; }
        public string? USER_ROLE { get; set; }
        public DateTime? EFFECTIVE_START { get; set; }
        public DateTime? EFFECTIVE_END { get; set; }
        public DateTime LAST_UPDATED_TIMESTAMP  { get; set; }
        public string LAST_UPDATED_USERID { get; set; }

    }
}
