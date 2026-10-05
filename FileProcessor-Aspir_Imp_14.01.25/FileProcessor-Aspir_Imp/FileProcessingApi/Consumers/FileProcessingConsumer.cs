
using FileProcessingApi.Models;
using FileProcessingApi.ZnodeImport;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using System.Threading.Tasks;

namespace FileProcessingApi.Consumers
{
    public class FileProcessingConsumer : IConsumer<FileProcessingMessage>
    {
        
        private readonly IZnodeImport _znodeImport;

        public FileProcessingConsumer( IZnodeImport znodeImport)
        {
            _znodeImport = znodeImport;
        }

        public async Task Consume(ConsumeContext<FileProcessingMessage> context)
        {
            var message = context.Message;
            //Type? T = Assembly.GetExecutingAssembly().GetType($"FileProcessingApi.Models.{message.ModelType}");
            //Type? modelType = Type.GetType($"FileProcessingApi.Models.{message.ModelType}");

            //IFileProcessor fileProcessor = _fileProcessorFactory.CreateFileProcessor(message.FileType, message.LocationType, message.ModelType);//, Assembly.GetExecutingAssembly().GetType($"FileProcessingApi.Models.{message.ModelType}"));
           
            await _znodeImport.ImportData(message);
           
        }
        
    }
}
