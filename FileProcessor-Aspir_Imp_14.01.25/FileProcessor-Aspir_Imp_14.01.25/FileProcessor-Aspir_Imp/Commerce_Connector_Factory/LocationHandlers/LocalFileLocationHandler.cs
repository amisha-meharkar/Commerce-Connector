using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_Factory.LocationHandlers
{
    public class LocalFileLocationHandler : IFileLocationHandler
    {
        public Stream GetFileStream(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file does not exist.", filePath);
            }

            var stream = new MemoryStream();
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(stream);
            }

            stream.Position = 0;
            return stream;
        }

        public async Task<Stream> GetFileStreamAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file does not exist.", filePath);
            }

            var stream = new MemoryStream();
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                await fileStream.CopyToAsync(stream);
            }

            stream.Position = 0;
            return stream;
        }
    }
}
