using Commerce_Connector_BAL.IAgent;
using Commerce_Connector_Factory.Factories;
using Commerce_Connector_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_BAL.Agent
{
    public class JobsAgent : IJobsAgent
    {
        private readonly HttpClient _httpClient;
        private readonly IFileProcessorFactory _fileProcessorFactory;

        public JobsAgent(IHttpClientFactory httpClientFactory, IFileProcessorFactory fileProcessorFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIHandler");
            _fileProcessorFactory = fileProcessorFactory;
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
    }
}
