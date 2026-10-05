using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_ViewModel.ViewModels
{
    public class FileDataViewModel
    {
        public List<string> Headers { get; set; }  
        public List<Dictionary<string, string>> Rows { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

}
