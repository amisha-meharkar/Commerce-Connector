using System.ComponentModel.DataAnnotations;

namespace FileProcessingApi.Models
{
    public class ImportModel
    {
        [Required]
        public string FileName { get; set; }

        [Required]
        public string ImportType { get; set; }

        public int ImportTypeId { get; set; }
        public ImportMappingListModel Mappings { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public int? FamilyId { get; set; }
        public bool IsPartialPage { get; set; }
        public int? PriceListId { get; set; }
        public string CountryCode { get; set; }
        public int? CatalogId { get; set; }
        public int? PromotionTypeId { get; set; }

        public byte[] ImportData { get; set; }
    }
}
