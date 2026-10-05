namespace FileProcessingApi.Models
{
    public class TemplateDetails
    {
        public int Id {  get; set; }
        public int ImportTemplateId { get; set; }
        public string? TemplateName { get; set; }
        public int ImportHeadId { get; set; }
        public int FamilyId { get; set; }
        public int PromotionTypeId { get; set; }
    }
}
