using Commerce_Connector_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_Factory.LocationHandlers
{
    public class ApiFileLocationHandler : IFileLocationHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;
        //private readonly Dictionary<string, CachedFile> _fileCache = new Dictionary<string, CachedFile>();
        //private readonly string _apiKey;

        public ApiFileLocationHandler(IHttpClientFactory httpClientFactory, string baseUrl)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = baseUrl;
            //_apiKey = apiKey;
        }

        public Stream GetFileStream(string filePath)
        {
            var stream = new MemoryStream();
            var httpClient = _httpClientFactory.CreateClient(); // Use HttpClientFactory here

            var task = Task.Run(async () =>
            {
                var response = await httpClient.GetAsync($"{_baseUrl}/{filePath}");
                response.EnsureSuccessStatusCode();
                await response.Content.CopyToAsync(stream);
            });
            task.Wait();
            stream.Position = 0;
            return stream;
        }

        public async Task<Stream> GetFileStreamAsync(string filePath)
        {
            var stream = new MemoryStream();
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"{_baseUrl}/{filePath}");
            response.EnsureSuccessStatusCode();
            await response.Content.CopyToAsync(stream);

            stream.Position = 0;
            return stream;
        }

        //public async Task<Stream> GetFileStreamAsync(string filePath)
        //{
        //    try
        //    {
        //        var stream = new MemoryStream();
        //        var httpClient = _httpClientFactory.CreateClient();

        //        // Check if the file is cached
        //        if (_fileCache.ContainsKey(filePath))
        //        {
        //            var cachedFile = _fileCache[filePath];
        //            var response = await httpClient.GetAsync($"{_baseUrl}/{filePath}");

        //            // Ensure response is successful and check for 'Last-Modified' header
        //            response.EnsureSuccessStatusCode();

        //            // Check if the server's Last-Modified header matches the cached file's metadata
        //            if (response.Headers.TryGetValues("Last-Modified", out var values))
        //            {
        //                var lastModified = values.First();
        //                if (lastModified == cachedFile.LastModified)
        //                {
        //                    // If the file has not changed, return the cached file
        //                    cachedFile.Stream.Position = 0; // Reset the stream position
        //                    return cachedFile.Stream;
        //                }
        //            }
        //        }

        //        // If file is not cached or has changed, download the file
        //        var downloadResponse = await httpClient.GetAsync($"{_baseUrl}/{filePath}");
        //        downloadResponse.EnsureSuccessStatusCode();
        //        await downloadResponse.Content.CopyToAsync(stream);

        //        // Reset stream position after download
        //        stream.Position = 0;

        //        // Cache the new file and its Last-Modified header
        //        var newCachedFile = new CachedFile
        //        {
        //            Stream = stream,
        //            LastModified = downloadResponse.Headers.Contains("Last-Modified")
        //                ? downloadResponse.Headers.GetValues("Last-Modified").FirstOrDefault() // Cache the Last-Modified header
        //                : null // If Last-Modified header is missing, set to null
        //        };

        //        _fileCache[filePath] = newCachedFile; // Update cache

        //        return stream;
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        // Log HTTP request errors
        //        Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
        //        throw; // Optionally rethrow to propagate the exception
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log any other errors (general exceptions)
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //        throw; // Optionally rethrow to propagate the exception
        //    }
        //}


    }
}
