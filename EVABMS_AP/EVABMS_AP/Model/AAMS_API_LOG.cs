namespace EVABMS_AP.Model
{
    public class AAMS_API_LOG
    {
        public long ID { get; set; }
        public string FUNCTION { get; set; }
        public string INPUT_DATA { get; set; }
        public string SUCC { get; set; }
        public string? MSG { get; set; }
        public DateTime LAST_UPDATED_TIMESTAMP { get; set; }
        public long? FK_AAML_SEQ { get; set; }
    }
}
