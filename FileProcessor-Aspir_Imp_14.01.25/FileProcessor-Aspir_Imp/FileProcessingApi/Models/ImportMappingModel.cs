namespace FileProcessingApi.Models
{
    public class ImportMappingModel
    {
        public int MappingId { get; set; }
        public string MapCsvColumn { get; set; }
        public string MapTargetColumn { get; set; }
        public string TransitionRule { get; set; }
    }
}
