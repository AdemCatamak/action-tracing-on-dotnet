namespace WebApplication.TraceInfoSection
{
    public interface ITraceInfo
    {
        string TraceId { get; set; }
        string MachineName { get; set; }
    }
    
    public class TraceInfo : ITraceInfo
    {
        public string TraceId { get; set; }
        public string MachineName { get; set; }
    }
}