using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_ViewModel.ViewModels
{
    public class MappingViewModel
    {
        public int MappingId { get; set; }
        public string? MapCsvColumn { get; set; }
        public string? MapTargetColumn { get; set; }
        public string? TransitionExpression { get; set; }
    }
}
