using Commerce_Connector_Factory.LocationHandlers;
using Commerce_Connector_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_Factory.Factories
{
    public class FileLocationHandlerFactory : IFileLocationHandlerFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FileLocationHandlerFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IFileLocationHandler CreateFileLocationHandler(string locationType, LocationConfiguration locationConfig)
        {
            return locationType.ToLower() switch
            {
                "sftp" => new SftpFileLocationHandler(locationConfig.ServerAddress, locationConfig.Username, locationConfig.Password),
                "blob" => new BlobFileLocationHandler(locationConfig.ConnectionString, locationConfig.ContainerName),
                "api" => new ApiFileLocationHandler(_httpClientFactory, locationConfig.ApiEndpoint),
                "local" => new LocalFileLocationHandler(),
                "ftp" => new FtpFileLocationHandler(locationConfig.ServerAddress, locationConfig.Username, locationConfig.Password),
                _ => throw new ArgumentException("Invalid location type", nameof(locationType)),
            };
        }
    }
}
