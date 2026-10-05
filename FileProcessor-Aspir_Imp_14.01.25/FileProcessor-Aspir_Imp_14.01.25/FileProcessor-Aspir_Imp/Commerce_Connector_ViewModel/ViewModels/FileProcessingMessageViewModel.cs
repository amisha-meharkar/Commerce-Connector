namespace Commerce_Connector_ViewModel.ViewModels
{
    public class FileProcessingMessageViewModel
    {
        public int? JobId { get; set; }
        public string? FilePath { get; set; }
        public string? FileType { get; set; }
        public string? LocationType { get; set; }
        public string? ModelType { get; set; }
        public List<string> CsvHeaders { get; set; } = new List<string>();
        public LocationConfiguration? LocationConfig { get; set; }
        public string? JobType { get; set; }
        public TemplateDetails? TemplateDetails { get; set; }
        public string? MappingsJson { get; set; }
        public List<MappingViewModel> Mappings { get; set; } = new List<MappingViewModel>();
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
