namespace FileProcessingApi.Models
{
    public class JobScheduleModel
    {
        public string JobSchedule { get; set; }
        public string CronExpression { get; set; }
        public DateTime? JobTime { get; set; }
    }
}
