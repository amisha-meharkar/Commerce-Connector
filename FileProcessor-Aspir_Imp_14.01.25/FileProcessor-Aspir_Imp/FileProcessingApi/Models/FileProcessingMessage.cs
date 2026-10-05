namespace FileProcessingApi.Models
{
    public class FileProcessingMessage
    {
        public int? JobId { get; set; }
        public string? FilePath { get; set; }
        public string? FileType { get; set; }
        public string? LocationType { get; set; }
        public string? ModelType { get; set; } // Use to identify the model for processing
        public string? JobType { get; set; }
        public string? MappingsJson { get; set; }
       //public Dictionary<string, string> MappingsJson { get; set; } 
        public LocationConfiguration? LocationConfig { get;  set; }
        public TemplateDetails? TemplateDetails { get; set; }
    }
    public class LocationConfiguration
    {
        public string? ServerAddress { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ApiEndpoint { get; set; }
        public string? ApiKey { get; set; }
        public string? ContainerName { get; set; }
        public string? ConnectionString { get; set; }
    }
}
