using Commerce_Connector_BAL.IAgent;
using Commerce_Connector_Factory.Factories;
using Commerce_Connector_ViewModel.ViewModels;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Json;
using System.Text;
using System.Xml.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using IFileProcessor = Commerce_Connector_Factory.FileProcessors.IFileProcessor;

namespace Commerce_Connector_BAL.Agent
{
    public class FileProcessingAgent : IFileProcessingAgent
    {
        private readonly HttpClient _httpClient;
        private readonly IFileProcessorFactory _fileProcessorFactory;

        public FileProcessingAgent(IHttpClientFactory httpClientFactory, IFileProcessorFactory fileProcessorFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIHandler");
            _fileProcessorFactory = fileProcessorFactory;
        }

        public async Task<HttpResponseMessage> ProcessFileData(FileProcessingMessageViewModel message)
        {
            try
            {
                return await _httpClient.PostAsJsonAsync("https://localhost:7185/api/FileProcessing/ProcessFile", message);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error while processing file data: " + ex.Message, ex);
            }
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

        public async Task<HttpResponseMessage> ScheduleRecurringJobAsync(string cronExpression)
        {
            try
            {
                string url = $"https://localhost:7185/api/FileProcessing/CreateRecurringJob?cronExpression={Uri.EscapeDataString(cronExpression)}";
                return await _httpClient.PostAsync(url, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while scheduling recurring job: " + ex.Message, ex);
            }
        }

        public async Task<HttpResponseMessage> ScheduleOneTimeJobAsync(DateTime jobTime)
        {
            try
            {
                string url = $"https://localhost:7185/api/FileProcessing/CreateOneTimeJob?jobTime={Uri.EscapeDataString(jobTime.ToString("o"))}";
                return await _httpClient.PostAsync(url, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while scheduling one-time job: " + ex.Message, ex);
            }
        }

        public async Task<HttpResponseMessage> ImmediateJobAsync(FileProcessingMessageViewModel fileMessage)
        {
            try
            {
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(fileMessage);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await _httpClient.PostAsync("https://localhost:7185/api/FileProcessing/ImmidiateJob", content);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving file message: " + ex.Message, ex);
            }
        }
        public async Task<List<FileProcessingMessageViewModel>> GetJobListAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7185/api/FileProcessing/GetJobDetails");
                response.EnsureSuccessStatusCode();  // Throws an exception if the response code is not 2xx
                string data = await response.Content.ReadAsStringAsync();

                List<FileProcessingMessageViewModel> result = JsonConvert.DeserializeObject<List<FileProcessingMessageViewModel>>(data);
                return result;
            }
            catch (Exception ex)
            {
                // Handle exception here (e.g., log it)
                throw new ApplicationException("An error occurred while retrieving job list", ex);
            }
        }

        public async Task<FileProcessingMessageViewModel> GetJobById(int id)
        {
            try
            {
                // Define the API URL with the job ID
                HttpResponseMessage response = await _httpClient.GetAsync($"https://localhost:7185/api/FileProcessing/GetJobById/{id}");
                response.EnsureSuccessStatusCode(); // Ensure the response is successful (2xx status code)

                // Read the content as a string
                string data = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string to the desired model
                FileProcessingMessageViewModel result = JsonConvert.DeserializeObject<FileProcessingMessageViewModel>(data);
                return result;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log it or wrap it in a custom exception)
                throw new ApplicationException($"An error occurred while retrieving job details for ID {id}", ex);
            }
        }


        public async Task<HttpResponseMessage> UpdateJob(FileProcessingMessageViewModel fileMessage)
        {
            try
            {
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(fileMessage);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                return await _httpClient.PostAsync("https://localhost:7185/api/FileProcessing/UpdateJob", content);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving file message: " + ex.Message, ex);
            }
        }

        public async Task<DataTable> GetPreviewData(FileProcessingMessageViewModel fileMessage)
        {
            try
            {
                IFileProcessor fileProcessor = (IFileProcessor)_fileProcessorFactory.CreateFileProcessor(
                    fileMessage.FileType,
                    fileMessage.LocationType,
                    fileMessage.ModelType,
                    fileMessage.LocationConfig
                );

                if (fileProcessor != null)
                {
                    // Get the transformed data as DataTable
                    var dataTable = await fileProcessor.ShowTransformedDataView(fileMessage);

                    return dataTable;  // Return the DataTable directly
                }
                else
                {
                    throw new Exception($"No file processor found for type {fileMessage.FileType}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing file data: " + ex.Message, ex);
            }
        }

        public async Task<FileDataViewModel> DisplayFileAsync(FileProcessingMessageViewModel fileMessage)
        {
            try
            {
                // Declare fileDataViewModel once
                FileDataViewModel fileDataViewModel = new FileDataViewModel(); // Initialize the variable outside the if block

                IFileProcessor fileProcessor = (IFileProcessor)_fileProcessorFactory.CreateFileProcessor(
                    fileMessage.FileType,
                    fileMessage.LocationType,
                    fileMessage.ModelType,
                    fileMessage.LocationConfig
                );

                if (fileProcessor != null)
                {
                    // Assign the result to fileDataViewModel
                    fileDataViewModel = await fileProcessor.DisplayFile(fileMessage.FilePath);

                }
                else
                {
                    throw new Exception($"No file processor found for type {fileMessage.FileType}");
                }

                // Return the ViewModel
                return fileDataViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing file data: " + ex.Message, ex);
            }
        }


    }

}
