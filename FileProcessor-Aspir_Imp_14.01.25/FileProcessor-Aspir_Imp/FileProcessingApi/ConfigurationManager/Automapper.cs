using Znode.Engine.Api.Models;
using AutoMapper;



namespace FileProcessingApi.ConfigurationManager
{
   
        public class Automapper : Profile
        {
            /*public Automapper()
            {
                // Map dynamic (CSV records) to InventoryModel
                CreateMap<dynamic, InventoryModel>()
                    .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => (string)src.WarehouseCode))  // Direct mapping for WarehouseName
                    .ForMember(dest => dest.ImportInventoryList, opt => opt.MapFrom(src => new List<ImportInventoryModel>
                    {
                    new ImportInventoryModel
                    {
                        SKU = src.SKU,
                        Quantity = src.Quantity,
                        ReOrderLevel = src.ReOrderLevel,
                        RowNumber = 1, // Static value or can be dynamically assigned
                        ListCode = src.ListCode, // Assuming this field is present in CSV
                        ListName = src.ListName  // Assuming this field is present in CSV
                    }
                    }));

                // Map dynamic (CSV fields) to ImportInventoryModel (for nested model)
                CreateMap<dynamic, ImportInventoryModel>()
                    .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => (string)src.SKU))
                    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => (int)src.Quantity))
                    .ForMember(dest => dest.ReOrderLevel, opt => opt.MapFrom(src => (int)src.ReOrderLevel));
            }*/
        }
    
}
