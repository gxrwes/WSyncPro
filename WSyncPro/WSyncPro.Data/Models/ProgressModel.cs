namespace WSyncPro.Data.Models
{
    public class ProgressModel
    {
        public string ServiceName { get; set; }          // Name of the service reporting the progress
        public string Status { get; set; }               // Current status message
        public int PercentageComplete { get; set; }      // Percentage of task completed
        public DateTime Timestamp { get; set; }          // Timestamp of the progress report

        public ProgressModel(string serviceName, string status, int percentageComplete)
        {
            ServiceName = serviceName;
            Status = status;
            PercentageComplete = percentageComplete;
            Timestamp = DateTime.Now;
        }
    }
}
