namespace EVABMS_AP.Model
{
    public class MGT_LOTTERY_PRIZE
    {
        public long ID { get; set; }
        public long FK_MLOD_ID { get; set; }
        public string NAME  { get; set; }
        public long NUMBERS { get; set; }
        public DateTime LAST_UPDATED_TIMESTAMP { get; set; }
        public string LAST_UPDATED_USERID { get; set; }
    }
}
