

using AdsTestService.PlcStructure;

namespace AdsTestService.Model
{
    public class WorkItem
    {
        public short ItemId { get; set; }
        public string pcTimeStamp { get; set; } = string.Empty;
        public string plcTimeStamp { get; set; } = string.Empty;    
        public TimeSpan timeSpan { get; set; }
    }
}
