using Commerce_Connector_BAL.IAgent;
using Commerce_Connector_ViewModel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CommerceConnector.Controllers
{
    public class JobsController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IJobsAgent _jobsAgent;
        private readonly IFileProcessingAgent _fileProcessingAgent;

        public JobsController(IHttpClientFactory httpClientFactory, IJobsAgent jobsAgent, IFileProcessingAgent fileProcessingAgent)
        {
            _httpClient = httpClientFactory.CreateClient("APIHandler");
            _jobsAgent = jobsAgent;
            _fileProcessingAgent = fileProcessingAgent;
        }


        [HttpPost]
        public async Task<IActionResult> Schedule(string jobSchedule, DateTime jobTime, string cronExpression)
        {
            if (jobSchedule == "recurring" && !string.IsNullOrEmpty(cronExpression))
            {
                // Call the agent to schedule a recurring job
                HttpResponseMessage response = await _jobsAgent.ScheduleRecurringJobAsync(cronExpression);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Recurring job scheduled successfully!");
                }
            }
            else if (jobSchedule == "oneTime")
            {
                // Call the agent to schedule a one-time job
                HttpResponseMessage response = await _jobsAgent.ScheduleOneTimeJobAsync(jobTime);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("One-time job scheduled successfully!");
                }
            }

            return BadRequest("Invalid job schedule type or missing required data.");
        }
        //[HttpGet]
        //public async Task<ActionResult> JobsList()
        //{
        //    List<FileProcessingMessageViewModel> jobsList = await _fileProcessingAgent.GetJobListAsync();
        //    return View(jobsList);
        //}

        [HttpGet]
        public async Task<ActionResult> JobsList()
        {
            List<FileProcessingMessageViewModel> jobsList = await _fileProcessingAgent.GetJobListAsync();

            return View(jobsList);
        }


        public async Task<ActionResult> Edit(int id)
        {
            var job = await _fileProcessingAgent.GetJobById(id);

            if (!string.IsNullOrEmpty(job.MappingsJson))
            {
                job.Mappings = JsonConvert.DeserializeObject<List<MappingViewModel>>(job.MappingsJson);
            }

            return View(job);
        }

        [HttpPost]
        public ActionResult Edit(FileProcessingMessageViewModel job)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (job.Mappings != null)
                    {
                        job.MappingsJson = JsonConvert.SerializeObject(job.Mappings);
                    }

                    _fileProcessingAgent.UpdateJob(job);
                    return RedirectToAction("JobsList");
                }
                catch (Exception ex)
                {
                    // Log the exception (e.g., using a logging framework)
                    ModelState.AddModelError("", "An error occurred while updating the job.");
                }
            }

            // If we reach here, something went wrong; return the view with the current job model
            return View(job);
        }

    }
}
