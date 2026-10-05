using FileProcessingApi.Models;
using Newtonsoft.Json;
using System.Text;
using ImportMappingListModel = Znode.Engine.Api.Models.ImportMappingListModel;
using ImportModel = Znode.Engine.Api.Models.ImportModel;
using ImportMappingModel = Znode.Engine.Api.Models.ImportMappingModel;


namespace FileProcessingApi.ZnodeImport
{
    public class ZnodeImport : IZnodeImport
    {
        public async Task ImportData(FileProcessingMessage message)
        {
            try
            {
                string filePath = string.Empty;

                // Check if the file location is local
                if (message.LocationType == "Local")
                {
                    // If location is local, use the provided file path
                    filePath = message.FilePath;
                }
                else
                {
                   
                    string localDirectoryPath = @"C:\Users\amisha.meharkar\Documents\FileProcessor-Aspir_Imp\FileProcessingApi\Sample";

                    
                    string fileName = Path.GetFileName(message.FilePath); 
                    filePath = Path.Combine(localDirectoryPath, fileName); 
                }

                // Now, use the appropriate file path to convert the record to import model
                var importModel = ConvertRecordToImportModel(filePath, message.MappingsJson, message.TemplateDetails, message.ModelType);

                // Send the model to the API
                await SendRecordToApiAsync(importModel); // Send the model to API
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        public ImportModel ConvertRecordToImportModel(string filePath, string? mappingsJson, TemplateDetails templateDetails, string modelType)
        {
            

            //var mappingJson = GetLatestMappingJsonFromDatabase(filePath);

            var model = new ImportModel
            {
                FileName = filePath,
                ImportType = modelType,
                ImportTypeId = templateDetails.ImportHeadId, // Replace with actual logic
                TemplateId = templateDetails.ImportTemplateId,
                LocaleId = 1,
                TemplateName = templateDetails.TemplateName,
                TemplateVersion = null,
                FamilyId = templateDetails.FamilyId,
                IsPartialPage = false,
                PriceListId = null,
                CountryCode = null,
                CatalogId = null,
                PromotionTypeId = templateDetails.PromotionTypeId,
                ImportData = File.ReadAllBytes(filePath),
                Mappings = new ImportMappingListModel
                {
                    // Deserialize JSON into DataMappings
                    DataMappings = JsonConvert.DeserializeObject<List<ImportMappingModel>>(mappingsJson)
                }

            };

            return model;
        }

       
        // Method to send the ImportModel to the API
        public async Task SendRecordToApiAsync(ImportModel model)
        {
            using (var httpClient = new HttpClient())
            {
                var apiUrl = "https://localhost:44375/Import/ImportData";  //Replace with actual API URL                                                                                                                       

                var jsonContent = JsonConvert.SerializeObject(model);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Record successfully sent to API.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to send record to API. Status Code: {response.StatusCode}, Error: {errorContent}");
                }
            }
        }
       
    }
}
