using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models;
using Commerce_Connector_Factory.Factories;
using Commerce_Connector_BAL.IAgent;

namespace Commerce_Connector_BAL.Agent
{
    public class TemplateAgent: ITemplateAgent
    {
        private readonly HttpClient _httpClient;
        private readonly IFileProcessorFactory _fileProcessorFactory;

        public TemplateAgent(IHttpClientFactory httpClientFactory, IFileProcessorFactory fileProcessorFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIHandler");
            _fileProcessorFactory = fileProcessorFactory;
        }

        public async Task<List<ImportTemplateModel>> GetTemplatesAsync(int importHeadId, int familyId, int promotionTypeId = 0)
        {
            try
            {
                string url = $"https://localhost:44375/Import/GetAllTemplates/{importHeadId}/{familyId}/{promotionTypeId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                ImportResponse templates = JsonConvert.DeserializeObject<ImportResponse>(content);

                return templates?.Import?.TemplateList?.TemplateList ?? new List<ImportTemplateModel>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to fetch templates: " + ex.Message, ex);
            }
        }

    }
}
