using FileProcessingApi.Models;
using ImportMappingListModel = Znode.Engine.Api.Models.ImportMappingListModel;
using ImportModel = Znode.Engine.Api.Models.ImportModel;
using ImportMappingModel = Znode.Engine.Api.Models.ImportMappingModel;


namespace FileProcessingApi.ZnodeImport
{
    public interface IZnodeImport
    {
        Task ImportData(FileProcessingMessage message);
        ImportModel ConvertRecordToImportModel(string filePath, string? mappingsJson, TemplateDetails templateDetails, string modelType);
        Task SendRecordToApiAsync(ImportModel model);


    }
}
