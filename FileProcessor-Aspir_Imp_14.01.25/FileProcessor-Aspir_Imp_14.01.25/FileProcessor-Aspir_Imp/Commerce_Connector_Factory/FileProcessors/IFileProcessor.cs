using Commerce_Connector_ViewModel.ViewModels;
using System.Data;

namespace Commerce_Connector_Factory.FileProcessors
{
    public interface IFileProcessor
    {
        Task<List<string>> ProcessFileAsync(string filePath);

        Task<FileDataViewModel> DisplayFile(string filePath);
        Task<DataTable> ShowTransformedDataView(FileProcessingMessageViewModel filemessage);

    }
}
