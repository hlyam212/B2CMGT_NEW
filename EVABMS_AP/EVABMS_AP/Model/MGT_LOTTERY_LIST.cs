namespace EVABMS_AP.Model
{
    public class MGT_LOTTERY_LIST
    {
        public long ID { get; set; }
        public long FK_MLOD_ID { get; set; }
        public string LIST_CONTENT_ENC { get; set; }
        public long FK_MLOP_ID { get; set; }
        public DateTime WINNING_TIMESTAMP { get; set; }
        public DateTime LAST_UPDATED_TIMESTAMP { get; set; }
        public string LAST_UPDATED_USERID { get; set; }
    }
}
