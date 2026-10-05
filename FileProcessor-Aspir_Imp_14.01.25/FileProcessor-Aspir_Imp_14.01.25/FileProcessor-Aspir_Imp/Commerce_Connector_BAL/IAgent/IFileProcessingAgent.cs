using Commerce_Connector_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;

namespace Commerce_Connector_BAL.IAgent
{
    public interface IFileProcessingAgent
    {
        Task<FileDataViewModel> DisplayFileAsync(FileProcessingMessageViewModel fileMessage);
        Task<DataTable> GetPreviewData(FileProcessingMessageViewModel fileMessage);
        Task<HttpResponseMessage> UpdateJob(FileProcessingMessageViewModel fileMessage);
        Task<FileProcessingMessageViewModel> GetJobById(int id);
        Task<List<FileProcessingMessageViewModel>> GetJobListAsync();
        Task<HttpResponseMessage> ProcessFileData(FileProcessingMessageViewModel message);
    }
}
