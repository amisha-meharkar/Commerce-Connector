using Commerce_Connector_BAL.IAgent;
using Commerce_Connector_ViewModel.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CommerceConnector.Controllers
{
    public class FileProcessingController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IJobsAgent _jobAgent;
        private readonly IFileProcessingAgent _fileProcessAgent;
        public FileProcessingController(IHttpClientFactory httpClientFactory, IJobsAgent jobAgent, IFileProcessingAgent fileProcessAgent)
        {
            _httpClient = httpClientFactory.CreateClient("APIHandler");
            _jobAgent = jobAgent;
            _fileProcessAgent = fileProcessAgent;
        }
        public IActionResult Index()
        {
            return View(new FileProcessingMessageViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(FileProcessingMessageViewModel message)
        {
            HttpResponseMessage response = await _jobAgent.ImmediateJobAsync(message);

            if (response.IsSuccessStatusCode)
            {
                TempData["StatusMessage"] = "File processing request submitted successfully!";
            }
            return RedirectToAction("Index");
        }



        public async Task<ActionResult> PreviewTransitionExpression(FileProcessingMessageViewModel fileMessage)
        {
            var dataTable = await _fileProcessAgent.GetPreviewData(fileMessage);  // Get DataTable from agent layer

            // Return the DataTable as a JSON response
            return PartialView("_DataPostview", dataTable);
        }


        public async Task<IActionResult> DisplayFile(FileProcessingMessageViewModel message)
        {
            // Get the result (data + errors) from the service
            FileDataViewModel viewModel = await _fileProcessAgent.DisplayFileAsync(message);

            // Pass the view model to the view
            return View("_DisplayFileData", viewModel);
        }
    }
}


