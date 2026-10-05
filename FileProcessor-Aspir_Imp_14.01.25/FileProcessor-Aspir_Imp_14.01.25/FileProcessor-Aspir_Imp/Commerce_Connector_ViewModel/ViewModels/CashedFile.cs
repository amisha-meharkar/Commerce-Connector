using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_ViewModel.ViewModels
{
    public class CachedFile
    {
        public Stream Stream { get; set; }
        public string LastModified { get; set; } // Store the Last-Modified header for comparison
    }
}
