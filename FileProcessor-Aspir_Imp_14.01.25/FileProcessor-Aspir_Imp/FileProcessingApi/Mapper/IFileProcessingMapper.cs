using FileProcessingApi.Models;

namespace FileProcessingApi.Mapper
{
    public interface IFileProcessingMapper
    {
        void MapLocationConfiguration(FileProcessingMessage message, dynamic jobSpecificData);
        void MapTemplateDetails(FileProcessingMessage message, dynamic templateDetail);
        void MapRecentTemplateDetails(FileProcessingMessage message, dynamic templateDetail);
        Commerce_Connector_ViewModel.ViewModels.LocationConfiguration MapToViewModelLocationConfiguration(FileProcessingApi.Models.LocationConfiguration locationConfig);


    }
}
