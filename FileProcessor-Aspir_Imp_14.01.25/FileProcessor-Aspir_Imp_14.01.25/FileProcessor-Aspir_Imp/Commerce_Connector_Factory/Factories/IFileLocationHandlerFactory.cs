using Commerce_Connector_Factory.LocationHandlers;
using Commerce_Connector_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_Factory.Factories
{
    public interface IFileLocationHandlerFactory
    {
        IFileLocationHandler CreateFileLocationHandler(string locationType, LocationConfiguration locationConfig);
        
    }
}
