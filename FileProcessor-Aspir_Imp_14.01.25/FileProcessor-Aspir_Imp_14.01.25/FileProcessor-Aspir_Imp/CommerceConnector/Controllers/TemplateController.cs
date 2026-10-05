using Commerce_Connector_BAL.IAgent;
using Commerce_Connector_ViewModel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Znode.Engine.Api.Models.Responses;

namespace CommerceConnector.Controllers
{
    public class TemplateController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly ITemplateAgent _templateAgent;
        private readonly IMappingAgent _mappingAgent;

        public TemplateController(IHttpClientFactory httpClientFactory, IFileProcessingAgent fileProcessingAgent, ITemplateAgent templateAgent, IMappingAgent mappingAgent)
        {
            _httpClient = httpClientFactory.CreateClient("APIHandler");
            _templateAgent = templateAgent;
            _mappingAgent = mappingAgent;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates(int importHeadId, int familyId, int promotionTypeId = 0)
        {
            try
            {
                // Fetch the template list from the service
                var templateList = await _templateAgent.GetTemplatesAsync(importHeadId, familyId, promotionTypeId);

                // Return the template list as JSON
                return Json(templateList);
            }
            catch (Exception ex)
            {
                // Log error (if logging is implemented) and return a failure response
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetTemplateDataAsync(TemplateDetails templateDetails)
        {
            try
            {
                if (templateDetails == null)
                {
                    return BadRequest("Invalid template details.");
                }

                ImportResponse importResponse = await _mappingAgent.GetTemplateDataAsync(templateDetails);
                return Json(importResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to fetch template data: {ex.Message}");
            }
        }

    }
}
