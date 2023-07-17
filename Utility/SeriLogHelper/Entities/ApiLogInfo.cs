namespace SeriLogHelper.Entities
{
    public class ApiLogInfo : LogInfo
    {
        public string HttpMethod { get; set; }
        public string RequestUrl { get; set; }
        public string RequestIpAddress { get; set; }
        public int? ResponseStatusCode { get; set; }
    }
}
