using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_Factory.LocationHandlers
{
    public class BlobFileLocationHandler : IFileLocationHandler
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobFileLocationHandler(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }

        public Stream GetFileStream(string filePath)
        {
            // Implement Blob storage logic to get file stream
            var blobClient = new BlobClient(_connectionString, "import", filePath);
            var stream = new MemoryStream();
            blobClient.DownloadTo(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task<Stream> GetFileStreamAsync(string filePath)
        {
            var blobClient = new BlobClient(_connectionString, _containerName, filePath);
            var stream = new MemoryStream();

            try
            {
                await blobClient.DownloadToAsync(stream);
                stream.Position = 0;
                return stream; // The caller must dispose of this stream
            }
            catch (Exception ex)
            {
                stream.Dispose(); // Clean up the stream in case of an error
                throw; // Re-throw the exception for the caller to handle
            }
        }
    }
}
