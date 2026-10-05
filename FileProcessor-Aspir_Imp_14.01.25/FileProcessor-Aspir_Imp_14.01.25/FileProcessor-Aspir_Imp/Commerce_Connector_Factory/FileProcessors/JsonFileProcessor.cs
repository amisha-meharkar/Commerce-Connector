using Commerce_Connector_Factory.LocationHandlers;
using Commerce_Connector_Factory.Validator;
using Commerce_Connector_ViewModel.ViewModels;
using Newtonsoft.Json;
using System.Data;
using TransformTableData;
using Znode.Engine.Api.Models;


namespace Commerce_Connector_Factory.FileProcessors
{
    public class JsonFileProcessor<T> : IFileProcessor
    {
        private readonly IFileLocationHandler _fileLocationHandler;
        private readonly IExpressionEvaluator _expressionEvaluator;
        private readonly DataPreprocessingAndValidation _preprocessingAndValidator;

        public JsonFileProcessor(IFileLocationHandler fileLocationHandler, IExpressionEvaluator expressionEvaluator)
        {
            _fileLocationHandler = fileLocationHandler;
            _expressionEvaluator = expressionEvaluator;
            _preprocessingAndValidator = new DataPreprocessingAndValidation();
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
            // var errorMessages = new List<string>(); // To collect error messages
            var errorMessages = new HashSet<string>();

            using (var stream = await _fileLocationHandler.GetFileStreamAsync(filePath))
            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync(); // Read all content from the file
                var jsonArray = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json); // Deserialize JSON to list of dictionaries

                // Create an instance of the DynamicValidator
                var validator = new DataPreprocessingAndValidation();

                foreach (var row in jsonArray)
                {
                    // Apply validation on each row
                    var validationErrors = _preprocessingAndValidator.ValidateRow(row);
                    foreach (var error in validationErrors)
                    {
                        errorMessages.Add(error);
                    }

                    tableData.Add(row); // Add the validated row
                }
            }

            // Extract headers from the first row, if available
            var headers = tableData.FirstOrDefault()?.Keys.ToList() ?? new List<string>();

            //var preprocessedData = await _preprocessingAndValidator.PreprocessDataWithFlaskApi(tableData);
            // Create and return the ViewModel
            var viewModel = new FileDataViewModel
            {
                Headers = tableData.FirstOrDefault()?.Keys.ToList() ?? new List<string>(),  // Extract headers from first row
                Rows = tableData,  // Use preprocessed data
                ErrorMessages = errorMessages.ToList()
            };

            return viewModel;
        }

        public async Task<DataTable> ShowTransformedDataView(FileProcessingMessageViewModel fileMessage)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Read JSON file using a stream
                using (var stream = await _fileLocationHandler.GetFileStreamAsync(fileMessage.FilePath))
                using (var reader = new StreamReader(stream))
                {
                    string jsonContent = await reader.ReadToEndAsync();

                    // Deserialize JSON to a list of objects (assuming it's an array of JSON objects)
                    var jsonData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent);

                    if (jsonData != null && jsonData.Count > 0)
                    {
                        // Add columns based on the keys of the first object in the JSON
                        foreach (var key in jsonData[0].Keys)
                        {
                            dataTable.Columns.Add(key);
                        }

                        // Add rows
                        foreach (var row in jsonData)
                        {
                            var values = row.Values.Cast<object>().ToArray();
                            dataTable.Rows.Add(values);
                        }
                    }
                }

                // Deserialize the MappingsJson as a list of ImportMappingModel
                var Mappings = new ImportMappingListModel
                {
                    DataMappings = JsonConvert.DeserializeObject<List<ImportMappingModel>>(fileMessage.MappingsJson)
                };

                // Apply mappings and transition expressions
                if (Mappings.DataMappings != null)
                {
                    foreach (var mapping in Mappings.DataMappings)
                    {
                        if (!string.IsNullOrEmpty(mapping.TransitionExpression))
                        {
                            // Clean up the expression if necessary
                            string expression = mapping.TransitionExpression.Replace("columnName", mapping.MapCsvColumn);

                            // Check if the source column exists in the DataTable
                            if (dataTable.Columns.Contains(mapping.MapCsvColumn))
                            {
                                // Ensure the target column exists
                                if (!dataTable.Columns.Contains(mapping.MapTargetColumn))
                                {
                                    // Create a new column for the calculated transition result
                                    dataTable.Columns.Add(new DataColumn(mapping.MapTargetColumn, typeof(string)));
                                }

                                // Apply the expression to each row
                                foreach (DataRow row in dataTable.Rows)
                                {
                                    // Ensure the correct value is extracted from the DataRow and processed
                                    row[mapping.MapTargetColumn] = _expressionEvaluator.EvaluateTransitionExpression(expression, row);
                                }
                            }
                            else
                            {
                                // Log or handle the case where the source column does not exist
                                Console.WriteLine($"Column '{mapping.MapCsvColumn}' not found in DataTable.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing JSON data: " + ex.Message, ex);
            }

            return dataTable;
        }

       
    }
}
