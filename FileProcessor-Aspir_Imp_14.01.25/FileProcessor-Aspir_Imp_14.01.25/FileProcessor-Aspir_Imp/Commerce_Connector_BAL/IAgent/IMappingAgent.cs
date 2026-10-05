using Commerce_Connector_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Models.Responses;

namespace Commerce_Connector_BAL.IAgent
{
    public interface IMappingAgent
    {
        Task<ImportResponse> GetTemplateDataAsync(TemplateDetails templateDetails);
        Task<HttpResponseMessage> SaveTemplateDetailsAsync(TemplateDetails templateDetails);
        Task<HttpResponseMessage> SaveFileMessageAsync(FileProcessingMessageViewModel fileMessage);
        Task<List<string>> GetFileHeadersAsync(FileProcessingMessageViewModel fileMessage);
        Task<List<string>> GetTargetColumnsAsync(TemplateDetails templateDetails);
    }
}
