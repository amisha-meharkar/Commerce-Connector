using Commerce_Connector_BAL.IAgent;
using Commerce_Connector_ViewModel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Renci.SshNet.Messages;
using System.Text.RegularExpressions;
using Znode.Engine.Api.Models.Responses;

namespace CommerceConnector.Controllers
{
    public class FileProcessingController : Controller
    {
        private readonly ILogger<FileProcessingController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IFileProcessingAgent _fileProcessingAgent;


        public FileProcessingController(ILogger<FileProcessingController> logger, IHttpClientFactory httpClientFactory, IFileProcessingAgent fileProcessingAgent)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("APIHandler");
            _fileProcessingAgent = fileProcessingAgent;
        }

        public IActionResult Index()
        {
            return View(new FileProcessingMessageViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(FileProcessingMessageViewModel message)
        {
            HttpResponseMessage response = await _fileProcessingAgent.ImmediateJobAsync(message);

            if (response.IsSuccessStatusCode)
            {
                TempData["StatusMessage"] = "File processing request submitted successfully!";
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Schedule(string jobSchedule, DateTime jobTime, string cronExpression)
        {
            if (jobSchedule == "recurring" && !string.IsNullOrEmpty(cronExpression))
            {
                // Call the agent to schedule a recurring job
                HttpResponseMessage response = await _fileProcessingAgent.ScheduleRecurringJobAsync(cronExpression);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Recurring job scheduled successfully!");
                }
            }
            else if (jobSchedule == "oneTime")
            {
                // Call the agent to schedule a one-time job
                HttpResponseMessage response = await _fileProcessingAgent.ScheduleOneTimeJobAsync(jobTime);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("One-time job scheduled successfully!");
                }
            }

            return BadRequest("Invalid job schedule type or missing required data.");
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates(int importHeadId, int familyId, int promotionTypeId = 0)
        {
            try
            {
                // Fetch the template list from the service
                var templateList = await _fileProcessingAgent.GetTemplatesAsync(importHeadId, familyId, promotionTypeId);

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

                ImportResponse importResponse = await _fileProcessingAgent.GetTemplateDataAsync(templateDetails);
                return Json(importResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching template data");
                return BadRequest($"Failed to fetch template data: {ex.Message}");
            }
        }

        public async Task<ActionResult> FetchPropertiesAndHeadersCombined(FileProcessingMessageViewModel fileMessage, TemplateDetails templateDetails)
        {

            // Get the target columns and headers based on the provided fileMessage
            List<string> targetColumns = (List<string>)await _fileProcessingAgent.GetTargetColumnsAsync(templateDetails);

            var viewModel = new Models.ModelMappingViewModel
            {
                Properties = targetColumns,
                Headers = await _fileProcessingAgent.GetFileHeadersAsync(fileMessage), // Implement this method based on the fileMessage
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
            HttpResponseMessage response = await _fileProcessingAgent.SaveFileMessageAsync(fileMessage);

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
            HttpResponseMessage response = await _fileProcessingAgent.SaveTemplateDetailsAsync(templateDetails);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Template details saved successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to save template details" });
            }
        }
        [HttpGet]
        public async Task<ActionResult> JobsList()
        {
            List<FileProcessingMessageViewModel> jobsList = await _fileProcessingAgent.GetJobListAsync();
            return View(jobsList);
        }

        public async Task<ActionResult> Edit(int id)
        {
            FileProcessingMessageViewModel job = await _fileProcessingAgent.GetJobById(id);
            var viewModel = new FileProcessingMessageViewModel
            {
                // Map other properties
                Mappings = JsonConvert.DeserializeObject<List<Mapping>>(job.MappingsJson)
            };
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Edit(FileProcessingMessageViewModel job)
        {
            if (ModelState.IsValid)
            {
                _fileProcessingAgent.UpdateJob(job);
                return RedirectToAction("JobsList");
            }

            return View(job);
        }


        public async Task<ActionResult> PreviewTransitionExpression(FileProcessingMessageViewModel fileMessage)
        {
            var dataTable = await _fileProcessingAgent.GetPreviewData(fileMessage);  // Get DataTable from agent layer

            // Return the DataTable as a JSON response
            return PartialView("_DataPostview", dataTable);
        }


        public async Task<IActionResult> DisplayFile(FileProcessingMessageViewModel message)
        {
            // Get the result (data + errors) from the service
            FileDataViewModel viewModel = await _fileProcessingAgent.DisplayFileAsync(message);

            // Pass the view model to the view
            return View("_DisplayFileData",viewModel);
        }


    }
}


