using Commerce_Connector_ViewModel.ViewModels;

namespace CommerceConnector.Models
{
    public class ModelMappingViewModel
    {
        public List<string> Properties { get; set; }
        public List<string> Headers { get; set; }
        public FileProcessingMessageViewModel FileProcessingMessage { get; internal set; }
    } 
}
