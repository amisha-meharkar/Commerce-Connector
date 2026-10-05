using Commerce_Connector_ViewModel.ViewModels;
using Projects;

namespace Configuration_Manager
{
    public class AutoMapperConfig
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
            CreateMap<Commerce_Connector_Data.DataModel.TemplateDetail, TemplateDetails>()
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
        }
    }
}
