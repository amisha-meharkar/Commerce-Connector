using Commerce_Connector_Factory.LocationHandlers;
using System.Xml.Linq;
using Commerce_Connector_ViewModel.ViewModels;
using Commerce_Connector_Factory.Validator;
using Newtonsoft.Json;
using System.Data;
using System.Xml;
using Znode.Engine.Api.Models;
using TransformTableData;

namespace Commerce_Connector_Factory.FileProcessors
{
    public class XmlFileProcessor<T> : IFileProcessor
    {
        private readonly IFileLocationHandler _fileLocationHandler;
        private readonly IExpressionEvaluator _expressionEvaluator;
        private readonly DataPreprocessingAndValidation _preprocessingAndValidator;

        public XmlFileProcessor(IFileLocationHandler fileLocationHandler, IExpressionEvaluator expressionEvaluator)
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
                {
                    var xDocument = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None); // Load XML asynchronously
                    var firstElement = xDocument.Root.Elements().FirstOrDefault();

                    if (firstElement != null)
                    {
                        headers.AddRange(firstElement.Elements().Select(e => e.Name.LocalName)); // Extract element names
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: File {filePath} not found.");
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
            // var errorMessages = new List<string>();
            var errorMessages = new HashSet<string>();

            using (var stream = await _fileLocationHandler.GetFileStreamAsync(filePath))
            {
                var document = XDocument.Load(stream); // Load the XML file

                // Assuming each "Inventory" element in XML represents a row
                var rows = document.Descendants("Inventory");

                // Create an instance of the DynamicValidator
                var validator = new DataPreprocessingAndValidation();

                foreach (var row in rows)
                {
                    var rowData = new Dictionary<string, string>();

                    foreach (var column in row.Elements()) // Iterate over the child elements (columns)
                    {
                        rowData[column.Name.LocalName] = column.Value; // Get the column name and value
                    }

                    // Apply validation on each row
                    var validationErrors = _preprocessingAndValidator.ValidateRow(rowData);
                    foreach (var error in validationErrors)
                    {
                        errorMessages.Add(error);
                    }

                    tableData.Add(rowData); // Add the validated row data to the list
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
            var errorMessages = new List<string>();

            try
            {
                // Read XML file using a stream
                using (var stream = await _fileLocationHandler.GetFileStreamAsync(fileMessage.FilePath))
                {
                    var document = XDocument.Load(stream); // Load the XML file

                    // Assuming each "Inventory" element in XML represents a row
                    var rows = document.Descendants("Inventory");

                    // Create an instance of the DynamicValidator
                    var validator = new DataPreprocessingAndValidation();

                    bool isHeader = true;

                    foreach (var row in rows)
                    {
                        var rowData = new Dictionary<string, string>();

                        // Iterate over the child elements (columns) of each "Inventory" element
                        foreach (var column in row.Elements())
                        {
                            if (isHeader)
                            {
                                // Treat the first element names as column headers
                                dataTable.Columns.Add(column.Name.LocalName);  // Add column headers
                            }
                            rowData[column.Name.LocalName] = column.Value; // Add column name and its value
                        }

                        // Apply validation on each row
                        var validationErrors = validator.ValidateRow(rowData);
                        if (validationErrors.Any())
                        {
                            // Collect error messages for the row
                            errorMessages.AddRange(validationErrors);
                        }

                        // Add the row data to the DataTable
                        if (isHeader)
                        {
                            // After first row, set isHeader to false to prevent re-adding columns
                            isHeader = false;
                        }

                        // Convert the dictionary to an array of values and add to DataTable
                        var rowValues = new object[dataTable.Columns.Count];
                        foreach (var column in rowData)
                        {
                            int columnIndex = dataTable.Columns.IndexOf(column.Key);
                            if (columnIndex >= 0)
                            {
                                rowValues[columnIndex] = column.Value;
                            }
                        }

                        // Add the row to the DataTable
                        dataTable.Rows.Add(rowValues);
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
            catch (FileNotFoundException fileNotFoundEx)
            {
                // Specific error for file not found
                Console.WriteLine($"File not found: {fileNotFoundEx.Message}");
                throw new Exception("The specified file could not be found.", fileNotFoundEx);
            }
            catch (XmlException xmlEx)
            {
                // Specific error for XML parsing issues
                Console.WriteLine($"XML Parsing Error: {xmlEx.Message}");
                throw new Exception("An error occurred while parsing the XML file.", xmlEx);
            }
            catch (JsonException jsonEx)
            {
                // Specific error for JSON deserialization issues
                Console.WriteLine($"JSON Deserialization Error: {jsonEx.Message}");
                throw new Exception("An error occurred while deserializing the JSON mappings.", jsonEx);
            }
            catch (Exception ex)
            {
                // Generic error for any other unhandled exceptions
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                throw new Exception("An unexpected error occurred while processing the XML file data.", ex);
            }

            return dataTable;
        }

    }
}

