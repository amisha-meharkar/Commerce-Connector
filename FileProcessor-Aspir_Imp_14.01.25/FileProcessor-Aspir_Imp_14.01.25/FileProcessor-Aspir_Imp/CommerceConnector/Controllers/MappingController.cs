using Commerce_Connector_BAL.IAgent;
using Commerce_Connector_ViewModel.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CommerceConnector.Controllers
{
    public class MappingController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMappingAgent _mappingAgent;

        public MappingController(IHttpClientFactory httpClientFactory, IMappingAgent mappingAgent)
        {
            _httpClient = httpClientFactory.CreateClient("APIHandler");
            _mappingAgent = mappingAgent;
        }

        public async Task<ActionResult> FetchPropertiesAndHeadersCombined(FileProcessingMessageViewModel fileMessage, TemplateDetails templateDetails)
        {

            // Get the target columns and headers based on the provided fileMessage
            List<string> targetColumns = (List<string>)await _mappingAgent.GetTargetColumnsAsync(templateDetails);

            var viewModel = new Models.ModelMappingViewModel
            {
                Properties = targetColumns,
                Headers = await _mappingAgent.GetFileHeadersAsync(fileMessage), // Implement this method based on the fileMessage
                FileProcessingMessage = fileMessage

            };
            //ViewData["FileProcessingMessage"] = fileMessage;
            templateDetails = templateDetails;
            await SaveTemplateDetails(templateDetails);
            // Return the partial view with the model data
            return PartialView("_ModelMapping", viewModel);
        }


        [HttpPost]
        public async Task<ActionResult> SaveMappings(FileProcessingMessageViewModel fileMessage)
        {
            // Call the agent to save file message details
            HttpResponseMessage response = await _mappingAgent.SaveFileMessageAsync(fileMessage);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "File message saved successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to save file message" });
            }
        }

        // Action to save template details
        [HttpPost]
        public async Task<ActionResult> SaveTemplateDetails(TemplateDetails templateDetails)
        {
            // Call the agent to save template details
            HttpResponseMessage response = await _mappingAgent.SaveTemplateDetailsAsync(templateDetails);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Template details saved successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to save template details" });
            }
        }
    }
}
