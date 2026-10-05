
using Commerce_Connector_Factory.LocationHandlers;
using System.Xml.Linq;
using Commerce_Connector_Factory.TransformationLogic;
using Commerce_Connector_ViewModel.ViewModels;
using Commerce_Connector_Factory.Validator;

namespace Commerce_Connector_Factory.FileProcessors
{
    public class XmlFileProcessor<T> : IFileProcessor
    {
        private readonly IFileLocationHandler _fileLocationHandler;
        private readonly TransformationExpressionEvaluator _expressionEvaluator;

        public XmlFileProcessor(IFileLocationHandler fileLocationHandler)
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
            var errorMessages = new List<string>();

            using (var stream = await _fileLocationHandler.GetFileStreamAsync(filePath))
            {
                var document = XDocument.Load(stream); // Load the XML file

                // Assuming each "Inventory" element in XML represents a row
                var rows = document.Descendants("Inventory");

                // Create an instance of the DynamicValidator
                var validator = new DynamicValidator();

                foreach (var row in rows)
                {
                    var rowData = new Dictionary<string, string>();

                    foreach (var column in row.Elements()) // Iterate over the child elements (columns)
                    {
                        rowData[column.Name.LocalName] = column.Value; // Get the column name and value
                    }

                    // Apply validation on each row
                    var validationErrors = validator.ValidateRow(rowData);
                    if (validationErrors.Any())
                    {
                        // Collect error messages for the row
                        errorMessages.AddRange(validationErrors);
                    }

                    tableData.Add(rowData); // Add the validated row data to the list
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

        public Task<System.Data.DataTable> ShowTransformedDataView(FileProcessingMessageViewModel filemessage)
        {
            throw new NotImplementedException();
        }
    }
}
