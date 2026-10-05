using Commerce_Connector_Factory.FileProcessors;
using Commerce_Connector_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_Factory.Factories
{
    public interface IFileProcessorFactory
    {
        IFileProcessor CreateFileProcessor(string fileType, string locationType, string ModelType, LocationConfiguration locationConfig);

    }
}
