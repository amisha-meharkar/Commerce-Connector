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
        Task<HttpResponseMessage> ProcessFileData(FileProcessingMessageViewModel message);
       
        Task<ImportResponse> GetTemplateDataAsync(TemplateDetails templateDetails);
        Task<List<string>> GetTargetColumnsAsync(TemplateDetails templateDetails);
        Task<List<string>> GetFileHeadersAsync(FileProcessingMessageViewModel fileMessage);
        Task<HttpResponseMessage> SaveFileMessageAsync(FileProcessingMessageViewModel fileMessage);
        Task<List<ImportTemplateModel>> GetTemplatesAsync(int importHeadId, int familyId, int promotionTypeId = 0);
        Task<HttpResponseMessage> SaveTemplateDetailsAsync(TemplateDetails templateDetails);
        Task<HttpResponseMessage> ScheduleOneTimeJobAsync(DateTime jobTime);
        Task<HttpResponseMessage> ScheduleRecurringJobAsync(string cronExpression);
        Task<List<FileProcessingMessageViewModel>> GetJobListAsync();
        Task<FileProcessingMessageViewModel> GetJobById(int id);
        Task<HttpResponseMessage> UpdateJob(FileProcessingMessageViewModel fileMessage);
        Task<HttpResponseMessage> ImmediateJobAsync(FileProcessingMessageViewModel fileMessage);
        Task<DataTable> GetPreviewData(FileProcessingMessageViewModel fileMessage);
        Task<FileDataViewModel> DisplayFileAsync(FileProcessingMessageViewModel fileMessage);
        //        Task<string> ScheduleJob(string jobSchedule, DateTime jobTime, string cronExpression);
        //        Task<List<TemplateDetails>> GetTemplatesAsync(int importHeadId, int familyId, int promotionTypeId = 0);
        //        Task<ImportResponse> GetTemplateDataAsync(TemplateDetails templ
        //        ateDetails);
        //        Task<List<string>> GetTargetColumnsAsync(TemplateDetails templateDetails);
        //        List<string> GetFileHeaders(FileProcessingMessage fileMessage);
        //        Task<string> SaveTemplateDetailsAsync(TemplateDetails templateDetails);
        //        Task<string> SaveMappingsAsync(FileProcessingMessage fileMessage);
    }
}
