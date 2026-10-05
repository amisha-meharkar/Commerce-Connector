namespace FileProcessingApi.Models
{
    public class ImportMappingListModel
    {
        public ImportMappingListModel()
        {
            DataMappings = new List<ImportMappingModel>();
        }
        public List<ImportMappingModel> DataMappings { get; set; }


    }
}
