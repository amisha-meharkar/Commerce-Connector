using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_Factory.LocationHandlers
{
    public class SftpFileLocationHandler : IFileLocationHandler
    {
        private readonly string _host;
        private readonly string _username;
        private readonly string _password;

        public SftpFileLocationHandler(string host, string username, string password)
        {
            _host = host;
            _username = username;
            _password = password;
        }

        public Stream GetFileStream(string filePath)
        {
            // Implement SFTP logic to get file stream
            var sftpClient = new SftpClient(_host, _username, _password);
            sftpClient.Connect();
            var stream = new MemoryStream();
            sftpClient.DownloadFile(filePath, stream);
            sftpClient.Disconnect();
            stream.Position = 0;
            return stream;
        }

        public async Task<Stream> GetFileStreamAsync(string filePath)
        {
            return await Task.Run(() => GetFileStream(filePath));
        }
    }
}
