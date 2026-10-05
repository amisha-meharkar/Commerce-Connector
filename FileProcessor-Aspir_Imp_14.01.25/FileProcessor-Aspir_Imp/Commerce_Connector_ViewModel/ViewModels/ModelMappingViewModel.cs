using Commerce_Connector_ViewModel.ViewModels;

namespace CommerceConnector.Models
{
    public class ModelMappingViewModel
    {
        public int MappingId { get; set; }
        public string MapTargetColumn { get; set; }
        public string MapCsvColumn { get; set; }
        public string Expression { get; set; }

        
        public FileProcessingMessageViewModel FileProcessingMessage { get; set; }
    }
    
}

