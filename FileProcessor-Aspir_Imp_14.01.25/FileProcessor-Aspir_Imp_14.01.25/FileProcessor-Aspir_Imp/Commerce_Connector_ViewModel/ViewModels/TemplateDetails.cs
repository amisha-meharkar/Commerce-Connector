using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_ViewModel.ViewModels
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
