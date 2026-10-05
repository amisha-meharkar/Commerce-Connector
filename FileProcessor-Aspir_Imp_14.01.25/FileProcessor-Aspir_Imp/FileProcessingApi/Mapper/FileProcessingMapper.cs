using Commerce_Connector_Data.CommerceConnectorServices.IService;
using Commerce_Connector_Data.DataModel;
using FileProcessingApi.Models;
using LocationConfiguration = FileProcessingApi.Models.LocationConfiguration;

namespace FileProcessingApi.Mapper
{
    public class FileProcessingMapper : IFileProcessingMapper
    {
        public void MapLocationConfiguration(FileProcessingMessage message, dynamic jobSpecificData)
        {
            if (jobSpecificData != null)
            {
                message.LocationConfig = new LocationConfiguration
                {
                    ApiEndpoint = jobSpecificData[0].LocationConfig.ApiEndpoint,
                    ApiKey = jobSpecificData[0].LocationConfig.ApiKey,
                    ServerAddress = jobSpecificData[0].LocationConfig.ServerAddress,
                    Username = jobSpecificData[0].LocationConfig.Username,
                    Password = jobSpecificData[0].LocationConfig.Password,
                    ContainerName = jobSpecificData[0].LocationConfig.ContainerName,
                    ConnectionString = jobSpecificData[0].LocationConfig.ConnectionString
                };
                message.MappingsJson = jobSpecificData[0].MappingsJson;
            }
        }

        public void MapTemplateDetails(FileProcessingMessage message, dynamic templateDetail)
        {
            if (templateDetail != null )
            {
                message.TemplateDetails = new TemplateDetails()
                {
                    TemplateName = templateDetail[0].TemplateName,
                    ImportTemplateId = templateDetail[0].ImportTemplateId,
                    ImportHeadId = templateDetail[0].ImportHeadId,
                    FamilyId = templateDetail[0].FamilyId,
                    PromotionTypeId = templateDetail[0].PromotionTypeId
                };

            }
        }

        public void MapRecentTemplateDetails(FileProcessingMessage message, dynamic templateDetail)
        {
            if (templateDetail != null)
            {
                message.TemplateDetails = new TemplateDetails()
                {
                    TemplateName = templateDetail.TemplateName,
                    ImportTemplateId = templateDetail.ImportTemplateId,
                    ImportHeadId = templateDetail.ImportHeadId,
                    FamilyId = templateDetail.FamilyId,
                    PromotionTypeId = templateDetail.PromotionTypeId
                };

            }
        }
        public Commerce_Connector_ViewModel.ViewModels.LocationConfiguration MapToViewModelLocationConfiguration(FileProcessingApi.Models.LocationConfiguration locationConfig)
        {
            return new Commerce_Connector_ViewModel.ViewModels.LocationConfiguration
            {
                ServerAddress = locationConfig.ServerAddress,
                Username = locationConfig.Username,
                Password = locationConfig.Password,
                ApiEndpoint = locationConfig.ApiEndpoint,
                ApiKey = locationConfig.ApiKey,
                ContainerName = locationConfig.ContainerName,
                ConnectionString = locationConfig.ConnectionString

            };
        }
    }
}