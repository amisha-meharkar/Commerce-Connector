using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_Factory.LocationHandlers
{
    public interface IFileLocationHandler
    {
        Stream GetFileStream(string filePath);
        Task<Stream> GetFileStreamAsync(string filePath);
    }
}
