using Commerce_Connector_Factory.LocationHandlers;
using Commerce_Connector_Factory.Validator;
using Commerce_Connector_ViewModel.ViewModels;
using CommerceConnector.Models;
using Newtonsoft.Json;
using System.Data;
using System.Xml.Linq;
using TransformTableData;
using Znode.Engine.Api.Models;
using ImportMappingListModel = Znode.Engine.Api.Models.ImportMappingListModel;



namespace Commerce_Connector_Factory.FileProcessors
{
  
        public class CsvFileProcessor<T> : IFileProcessor
        {
        private readonly IFileLocationHandler _fileLocationHandler;
        private readonly DynamicValidator _validator;
        private readonly IExpressionEvaluator _expressionEvaluator;


        public CsvFileProcessor(IFileLocationHandler fileLocationHandler, IExpressionEvaluator expressionEvaluator)
        {
            _fileLocationHandler = fileLocationHandler;
            _expressionEvaluator = expressionEvaluator;
            _validator = new DynamicValidator();
        }

            public async Task<List<string>> ProcessFileAsync(string filePath)
            {
                var headers = new List<string>();

                using (var stream = await _fileLocationHandler.GetFileStreamAsync(filePath))
                //using (var stream = await _fileLocationHandler.GetFileStreamAsync(filePath))
                using (var reader = new StreamReader(stream))
                {
                    var line = await reader.ReadLineAsync(); // Read the first line asynchronously
                    if (line != null)
                    {
                        headers.AddRange(line.Split(',')); // Assuming comma as separator
                    }
                }

                return headers;
            }

       

        public async Task<FileDataViewModel> DisplayFile(string filePath)
        {
            var tableData = new List<Dictionary<string, string>>();
            var errorMessages = new List<string>();

            using (var stream = await _fileLocationHandler.GetFileStreamAsync(filePath))
            using (var reader = new StreamReader(stream))
            {
                var headers = new List<string>();
                string line = await reader.ReadLineAsync(); // Read the first line (headers)

                if (line != null)
                {
                    headers.AddRange(line.Split(',')); // Assuming comma as a separator

                    // Create an instance of the DynamicValidator
                    var validator = new DynamicValidator();

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var values = line.Split(',');
                        var row = new Dictionary<string, string>();

                        for (int i = 0; i < headers.Count; i++)
                        {
                            // Trim the double quotes from each value
                            row[headers[i]] = values.Length > i ? values[i].Trim('"') : null;
                        }

                        // Apply validation on each row
                        var validationErrors = validator.ValidateRow(row);
                        if (validationErrors.Any())
                        {
                            // Collect error messages for the row
                            errorMessages.AddRange(validationErrors);
                        }

                        tableData.Add(row);
                    }
                }
            }

            // Create and return the ViewModel containing both data and errors
            var viewModel = new FileDataViewModel
            {
                Headers = tableData.FirstOrDefault()?.Keys.ToList() ?? new List<string>(),  // Extract headers from first row
                Rows = tableData,
                ErrorMessages = errorMessages
            };

            return viewModel;  // Return the ViewModel
        }


        public async Task<DataTable> ShowTransformedDataView(FileProcessingMessageViewModel fileMessage)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Read CSV file using a stream
                using (var stream = await _fileLocationHandler.GetFileStreamAsync(fileMessage.FilePath))
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    bool isHeader = true;

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var values = line.Split(','); // Assuming CSV uses commas as delimiters

                        // Clean up unwanted characters like quotes or slashes
                        values = values.Select(value => value.Trim('"', '/')).ToArray();

                        if (isHeader)
                        {
                            // Add columns based on header row
                            foreach (var value in values)
                            {
                                dataTable.Columns.Add(value);
                            }
                            isHeader = false;
                        }
                        else
                        {
                            // Add rows
                            dataTable.Rows.Add(values.Cast<object>().ToArray());
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
                throw new Exception("Error processing file data: " + ex.Message, ex);
            }

            return dataTable;
        }

    }
}


