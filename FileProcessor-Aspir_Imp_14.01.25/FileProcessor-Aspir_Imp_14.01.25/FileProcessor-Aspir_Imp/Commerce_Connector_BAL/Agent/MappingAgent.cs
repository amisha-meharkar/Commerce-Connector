using Commerce_Connector_BAL.IAgent;
using Commerce_Connector_Factory.Factories;
using Commerce_Connector_Factory.FileProcessors;
using Commerce_Connector_ViewModel.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Models.Responses;

namespace Commerce_Connector_BAL.Agent
{
    public class MappingAgent : IMappingAgent
    {
        private readonly HttpClient _httpClient;
        private readonly IFileProcessorFactory _fileProcessorFactory;

        public MappingAgent(IHttpClientFactory httpClientFactory, IFileProcessorFactory fileProcessorFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIHandler");
            _fileProcessorFactory = fileProcessorFactory;
        }
        public async Task<List<string>> GetTargetColumnsAsync(TemplateDetails templateDetails)
        {
            try
            {
                ImportResponse importResponse = await GetTemplateDataAsync(templateDetails);

                return importResponse?.Import?.TemplateMappingList?.Mappings?
                    .Where(mapping => !string.IsNullOrEmpty(mapping.TargetColumnName))
                    .Select(mapping => mapping.TargetColumnName)
                    .ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error extracting TargetColumnNames: " + ex.Message, ex);
            }
        }

        public async Task<List<string>> GetFileHeadersAsync(FileProcessingMessageViewModel fileMessage)
        {
            try
            {
                var headers = new List<string>();
                IFileProcessor fileProcessor = (IFileProcessor)_fileProcessorFactory.CreateFileProcessor(
                    fileMessage.FileType,
                    fileMessage.LocationType,
                    fileMessage.ModelType,
                    fileMessage.LocationConfig
                );

                if (fileProcessor != null)
                {
                    headers = await fileProcessor.ProcessFileAsync(fileMessage.FilePath);
                }
                else
                {
                    throw new Exception($"No file processor found for type {fileMessage.FileType}");
                }

                return headers;
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing file headers: " + ex.Message, ex);
            }
        }

        public async Task<HttpResponseMessage> SaveFileMessageAsync(FileProcessingMessageViewModel fileMessage)
        {
            try
            {
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(fileMessage);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await _httpClient.PostAsync("https://localhost:7185/api/FileProcessing/SaveFileDetails", content);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving file message: " + ex.Message, ex);
            }
        }

        public async Task<HttpResponseMessage> SaveTemplateDetailsAsync(TemplateDetails templateDetails)
        {
            try
            {
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(templateDetails);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await _httpClient.PostAsync("https://localhost:7185/api/FileProcessing/SaveTemplateDetails", content);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving template details: " + ex.Message, ex);
            }
        }
        public async Task<ImportResponse> GetTemplateDataAsync(TemplateDetails templateDetails)
        {
            try
            {
                string url = $"https://localhost:44375/Import/GetTemplateData/" +
                             $"{templateDetails.ImportTemplateId}/" +
                             $"{templateDetails.ImportHeadId}/" +
                             $"{templateDetails.FamilyId}/" +
                             $"{templateDetails.PromotionTypeId}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ImportResponse>(content);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to fetch template data: " + ex.Message, ex);
            }
        }

    }
}
