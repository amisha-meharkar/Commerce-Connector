using Commerce_Connector_Factory.FileProcessors;
using Commerce_Connector_Factory.LocationHandlers;
using Commerce_Connector_ViewModel.ViewModels;
using System.Reflection;
using TransformTableData;


namespace Commerce_Connector_Factory.Factories
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        private readonly IFileLocationHandlerFactory _fileLocationHandlerFactory;
        private readonly IExpressionEvaluator _expressionEvaluator;

        public FileProcessorFactory(IFileLocationHandlerFactory fileLocationHandlerFactory, IExpressionEvaluator expressionEvaluator)
        {
            _fileLocationHandlerFactory = fileLocationHandlerFactory;
            _expressionEvaluator = expressionEvaluator;
        }
        //public IFileProcessor CreateFileProcessor<T>(string fileType, string locationType, Type modelType)
        public IFileProcessor CreateFileProcessor(string fileType, string locationType, string modelTypeName, LocationConfiguration locationConfig)
        {
            IFileLocationHandler fileLocationHandler = _fileLocationHandlerFactory.CreateFileLocationHandler(locationType, locationConfig);

            // Get the type from the modelTypeName string
            //Type modelType = Type.GetType(modelTypeName);
            //Type? modelType = Assembly.GetExecutingAssembly().GetType($"FileProcessingApi.Models.{modelTypeName}");
            // Load the external assembly containing models
            Assembly externalAssembly = Assembly.Load("Znode.Engine.Api.Models");
            //// Get the model type from the external assembly
            Type? modelType = externalAssembly.GetType($"Znode.Engine.Api.Models.{modelTypeName}");



            if (modelType == null)
            {
                throw new ArgumentException("Invalid model type", nameof(modelTypeName));
            }
            //if (modelType == typeof(Product))
            //{
            // Create the appropriate file processor instance using reflection
            var method = this.GetType().GetMethod(nameof(CreateFileProcessorGeneric), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var genericMethod = method.MakeGenericMethod(modelType);
            return (IFileProcessor)genericMethod.Invoke(this, new object[] { fileType, fileLocationHandler, _expressionEvaluator });

        }


        private IFileProcessor CreateFileProcessorGeneric<T>(string fileType, IFileLocationHandler fileLocationHandler,IExpressionEvaluator expressionEvaluator) where T : class
        {
            return fileType.ToLower() switch
            {
                "csv" => new CsvFileProcessor<T>(fileLocationHandler, expressionEvaluator),
                "json" => new JsonFileProcessor<T>(fileLocationHandler, expressionEvaluator),
                "xml" => new XmlFileProcessor<T>(fileLocationHandler, expressionEvaluator),
                _ => throw new ArgumentException("Invalid file type", nameof(fileType)),
            };
        }
    }
}
