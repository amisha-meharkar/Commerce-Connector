using AutoMapper;
using Commerce_Connector_Data.DataModel;
using FileProcessingApi.Models;


namespace FileProcessingApi.AutoMapperConfig
{
    public class FileProcessingProfile : Profile
    {
        public FileProcessingProfile()
        {
            // Mapping for LocationConfiguration
            CreateMap<Commerce_Connector_Data.DataModel.LocationConfiguration, FileProcessingApi.Models.LocationConfiguration>()
                .ForMember(dest => dest.ApiEndpoint, opt => opt.MapFrom(src => src.ApiEndpoint))
                .ForMember(dest => dest.ApiKey, opt => opt.MapFrom(src => src.ApiKey))
                .ForMember(dest => dest.ServerAddress, opt => opt.MapFrom(src => src.ServerAddress))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.ContainerName, opt => opt.MapFrom(src => src.ContainerName))
                .ForMember(dest => dest.ConnectionString, opt => opt.MapFrom(src => src.ConnectionString));

            // Mapping for TemplateDetails
            CreateMap<Commerce_Connector_Data.DataModel.TemplateDetail, FileProcessingApi.Models.TemplateDetails>()
                .ForMember(dest => dest.TemplateName, opt => opt.MapFrom(src => src.TemplateName))
                .ForMember(dest => dest.ImportTemplateId, opt => opt.MapFrom(src => src.ImportTemplateId))
                .ForMember(dest => dest.ImportHeadId, opt => opt.MapFrom(src => src.ImportHeadId))
                .ForMember(dest => dest.FamilyId, opt => opt.MapFrom(src => src.FamilyId))
                .ForMember(dest => dest.PromotionTypeId, opt => opt.MapFrom(src => src.PromotionTypeId));

            // Mapping to ViewModel LocationConfiguration
            CreateMap<FileProcessingApi.Models.LocationConfiguration, Commerce_Connector_ViewModel.ViewModels.LocationConfiguration>()
                .ForMember(dest => dest.ServerAddress, opt => opt.MapFrom(src => src.ServerAddress))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.ApiEndpoint, opt => opt.MapFrom(src => src.ApiEndpoint))
                .ForMember(dest => dest.ApiKey, opt => opt.MapFrom(src => src.ApiKey))
                .ForMember(dest => dest.ContainerName, opt => opt.MapFrom(src => src.ContainerName))
                .ForMember(dest => dest.ConnectionString, opt => opt.MapFrom(src => src.ConnectionString));

            CreateMap<TemplateDetails, Commerce_Connector_Data.DataModel.TemplateDetail>()
           .ForMember(dest => dest.ImportTemplateId, opt => opt.MapFrom(src => src.ImportTemplateId))
           .ForMember(dest => dest.TemplateName, opt => opt.MapFrom(src => src.TemplateName))
           .ForMember(dest => dest.ImportHeadId, opt => opt.MapFrom(src => src.ImportHeadId))
           .ForMember(dest => dest.FamilyId, opt => opt.MapFrom(src => src.FamilyId))
           .ForMember(dest => dest.PromotionTypeId, opt => opt.MapFrom(src => src.PromotionTypeId));

            CreateMap<FileProcessingMessage, Commerce_Connector_Data.DataModel.FileDetail>()
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath))
            .ForMember(dest => dest.MappingsJson, opt => opt.MapFrom(src => src.MappingsJson))
            .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
            .ForMember(dest => dest.ModelType, opt => opt.MapFrom(src => src.ModelType))
            .ForMember(dest => dest.LocationType, opt => opt.MapFrom(src => src.LocationType))
            .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.JobId)); // Assuming JobId exists in both models

            CreateMap<FileProcessingMessage, FileDetail>()
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath))
            .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
            .ForMember(dest => dest.LocationType, opt => opt.MapFrom(src => src.LocationType))
            .ForMember(dest => dest.ModelType, opt => opt.MapFrom(src => src.ModelType))
            .ForMember(dest => dest.MappingsJson, opt => opt.MapFrom(src => src.MappingsJson))
            .ForMember(dest => dest.LocationConfigId, opt => opt.MapFrom(src => new Random().Next(1, 100))); // Mapping for LocationConfigId

            // Mapping from LocationConfiguration to DataModel.LocationConfiguration
            CreateMap<Models.LocationConfiguration, Commerce_Connector_Data.DataModel.LocationConfiguration>()
                .ForMember(dest => dest.ServerAddress, opt => opt.MapFrom(src => src.ServerAddress))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.ApiEndpoint, opt => opt.MapFrom(src => src.ApiEndpoint))
                .ForMember(dest => dest.ApiKey, opt => opt.MapFrom(src => src.ApiKey))
                .ForMember(dest => dest.ContainerName, opt => opt.MapFrom(src => src.ContainerName))
                .ForMember(dest => dest.ConnectionString, opt => opt.MapFrom(src => src.ConnectionString));
        }
    }

}



