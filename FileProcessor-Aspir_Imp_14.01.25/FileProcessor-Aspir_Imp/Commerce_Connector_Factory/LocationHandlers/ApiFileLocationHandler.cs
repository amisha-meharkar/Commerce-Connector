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

    }
}
