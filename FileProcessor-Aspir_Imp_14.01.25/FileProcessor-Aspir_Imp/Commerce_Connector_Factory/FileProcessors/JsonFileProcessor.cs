using Commerce_Connector_Factory.LocationHandlers;
using Commerce_Connector_Factory.TransformationLogic;
using Commerce_Connector_Factory.Validator;
using Commerce_Connector_ViewModel.ViewModels;
using Newtonsoft.Json;
using System.Data;


namespace Commerce_Connector_Factory.FileProcessors
{
    public class JsonFileProcessor<T> : IFileProcessor
    {
        private readonly IFileLocationHandler _fileLocationHandler;
        private readonly TransformationExpressionEvaluator _expressionEvaluator;

        public JsonFileProcessor(IFileLocationHandler fileLocationHandler)
        {
            _fileLocationHandler = fileLocationHandler;
            _expressionEvaluator = new TransformationExpressionEvaluator();
        }

       
        public async Task<List<string>> ProcessFileAsync(string filePath)
        {
            var headers = new List<string>();

            try
            {
               
                using (var stream = await _fileLocationHandler.GetFileStreamAsync(filePath))
                using (var reader = new StreamReader(stream))
                {
                    var jsonData = await reader.ReadToEndAsync(); // Read the entire JSON content asynchronously

                    // Check if the file is empty
                    if (string.IsNullOrWhiteSpace(jsonData))
                    {
                        throw new Exception("JSON file is empty.");
                    }

                    // Deserialize JSON data
                    var jsonObjectList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData);
                    if (jsonObjectList != null && jsonObjectList.Count > 0)
                    {
                        headers.AddRange(jsonObjectList[0].Keys); // Extract headers from the first object
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: File {filePath} not found.");
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                Console.WriteLine($"Error: JSON deserialization issue. {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return headers;
        }

       

        public async Task<FileDataViewModel> DisplayFile(string filePath)
        {
            var tableData = new List<Dictionary<string, string>>();
            var errorMessages = new List<string>(); // To collect error messages

            using (var stream = await _fileLocationHandler.GetFileStreamAsync(filePath))
            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync(); // Read all content from the file
                var jsonArray = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json); // Deserialize JSON to list of dictionaries

                // Create an instance of the DynamicValidator
                var validator = new DynamicValidator();

                foreach (var row in jsonArray)
                {
                    // Apply validation on each row
                    var validationErrors = validator.ValidateRow(row);
                    if (validationErrors.Any())
                    {
                        // Collect error messages for the row
                        errorMessages.AddRange(validationErrors);
                    }

                    tableData.Add(row); // Add the validated row
                }
            }

            // Extract headers from the first row, if available
            var headers = tableData.FirstOrDefault()?.Keys.ToList() ?? new List<string>();

            // Create and return the ViewModel
            var viewModel = new FileDataViewModel
            {
                Headers = headers,
                Rows = tableData,
                ErrorMessages = errorMessages
            };

            return viewModel;
        }

        public Task<DataTable> ShowTransformedDataView(FileProcessingMessageViewModel filemessage)
        {
            throw new NotImplementedException();
        }
    }
}
