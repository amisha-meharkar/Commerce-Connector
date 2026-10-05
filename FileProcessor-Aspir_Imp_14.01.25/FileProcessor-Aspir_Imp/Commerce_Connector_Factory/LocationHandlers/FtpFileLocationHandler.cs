using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Commerce_Connector_Factory.LocationHandlers
{
    public class FtpFileLocationHandler : IFileLocationHandler
    {
        private readonly string _ftpHost;
        private readonly string _username;
        private readonly string _password;
        public FtpFileLocationHandler(string ftpHost, string username, string password)
        {
            _ftpHost = ftpHost;
            _username = username;
            _password = password;
        }

        //Without using fluentFTP .Net core native
        public Stream GetFileStreamNet(string filePath)
        {
            var request = (FtpWebRequest)WebRequest.Create(new Uri($"{_ftpHost}/{filePath}"));
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(_username, _password);

            // Get the response
            using (var response = (FtpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream == null)
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                var stream = new MemoryStream();
                responseStream.CopyTo(stream); // Copy the response stream to MemoryStream
                stream.Position = 0; // Reset position for reading
                return stream; // Return the stream
            }
        }
        public Stream GetFileStream(string filePath)
        {
            using (var ftpClient = new FtpClient(_ftpHost, _username, _password))
            {
                ftpClient.Connect();
                var stream = new MemoryStream();
                ftpClient.DownloadStream(stream, filePath);
                stream.Position = 0;
                return stream;
            }
        }

        //public async Task<Stream> GetFileStreamAsync(string filePath)
        //{
        //    var stream = new MemoryStream();
        //    var token = new CancellationToken();
        //    using (var ftpClient = new AsyncFtpClient(_ftpHost, _username, _password))
        //    {
        //        await ftpClient.Connect(token);
        //        await ftpClient.DownloadStream(stream, filePath);
        //        stream.Position = 0;
        //    }
        //    return stream;
        //}


        public async Task<Stream> GetFileStreamAsync(string ftpFilePath)
        {
            var stream = new MemoryStream();
            var token = new CancellationToken();

            // Hardcoded local directory path
            string localDirectoryPath = @"C:\Users\amisha.meharkar\Documents\FileProcessor-Aspir_Imp\FileProcessingApi\Sample";

            // Extract file name from FTP file path
            string fileName = Path.GetFileName(ftpFilePath);
            string localFilePath = Path.Combine(localDirectoryPath, fileName); // Construct full local file path

            try
            {
                using (var ftpClient = new AsyncFtpClient(_ftpHost, _username, _password))
                {
                    // Ensure the local folder exists
                    Directory.CreateDirectory(localDirectoryPath);

                    // Connect to FTP
                    try
                    {
                        await ftpClient.Connect(token);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error connecting to FTP server: {ex.Message}");
                    }

                    // Download file to local path with overwrite and verification options
                    try
                    {
                        await ftpClient.DownloadFile(localFilePath, ftpFilePath, FtpLocalExists.Overwrite, FtpVerify.None);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error downloading file from FTP: {ex.Message}");
                    }

                    // Load the file into a MemoryStream
                    try
                    {
                        using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
                        {
                            await fileStream.CopyToAsync(stream);
                        }

                        stream.Position = 0; // Reset stream for reading
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to read the downloaded file: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }

            return stream; // Return the memory stream
        }

    }
}
