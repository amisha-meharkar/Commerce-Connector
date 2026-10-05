using Commerce_Connector_Factory.LocationHandlers;
using Commerce_Connector_Factory.Validator;
using Commerce_Connector_ViewModel.ViewModels;
using Newtonsoft.Json;
using System.Data;
using TransformTableData;
using Znode.Engine.Api.Models;
using ImportMappingListModel = Znode.Engine.Api.Models.ImportMappingListModel;



namespace Commerce_Connector_Factory.FileProcessors
{

    public class CsvFileProcessor<T> : IFileProcessor
    {
        private readonly IFileLocationHandler _fileLocationHandler;
        private readonly DataPreprocessingAndValidation _preprocessingAndValidator;
        private readonly IExpressionEvaluator _expressionEvaluator;


        public CsvFileProcessor(IFileLocationHandler fileLocationHandler, IExpressionEvaluator expressionEvaluator)
        {
            _fileLocationHandler = fileLocationHandler;
            _expressionEvaluator = expressionEvaluator;
            _preprocessingAndValidator = new DataPreprocessingAndValidation();
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
            var errorMessages = new HashSet<string>(); // Use HashSet to avoid duplicate messages

            using (var stream = await _fileLocationHandler.GetFileStreamAsync(filePath))
            using (var reader = new StreamReader(stream))
            {
                var headers = new List<string>();
                string line = await reader.ReadLineAsync(); // Read the first line (headers)

                if (line != null)
                {
                    headers.AddRange(line.Split(',')); // Assuming comma as a separator


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
                        var validationErrors = _preprocessingAndValidator.ValidateRow(row);

                        // Add unique errors to the HashSet
                        foreach (var error in validationErrors)
                        {
                            errorMessages.Add(error);
                        }

                        tableData.Add(row);
                    }
                }
            }

            var viewModel = new FileDataViewModel
            {
                Headers = tableData.FirstOrDefault()?.Keys.ToList() ?? new List<string>(),  // Extract headers from first row
                Rows = tableData, // Use preprocessed data
                ErrorMessages = errorMessages.ToList()
            };
            return viewModel;
        }

        public async Task<DataTable> ShowTransformedDataView(FileProcessingMessageViewModel fileMessage)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Read CSV file using a stream
                try
                {
                    using (var stream = await _fileLocationHandler.GetFileStreamAsync(fileMessage.FilePath))
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        bool isHeader = true;

                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            try
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
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing line: {line}. Details: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error reading the file stream: " + ex.Message, ex);
                }

                // Deserialize the MappingsJson as a list of ImportMappingModel
                ImportMappingListModel Mappings;
                try
                {
                    Mappings = new ImportMappingListModel
                    {
                        DataMappings = JsonConvert.DeserializeObject<List<ImportMappingModel>>(fileMessage.MappingsJson)
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deserializing MappingsJson: " + ex.Message, ex);
                }

                // Apply mappings and transition expressions
                if (Mappings.DataMappings != null)
                {
                    foreach (var mapping in Mappings.DataMappings)
                    {
                        if (!string.IsNullOrEmpty(mapping.TransitionExpression))
                        {
                            try
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
                                        try
                                        {
                                            // Ensure the correct value is extracted from the DataRow and processed
                                            row[mapping.MapTargetColumn] = _expressionEvaluator.EvaluateTransitionExpression(expression, row);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error evaluating expression for row: {ex.Message}");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Column '{mapping.MapCsvColumn}' not found in DataTable.");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing mapping for column: {mapping.MapCsvColumn}. Details: {ex.Message}");
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


