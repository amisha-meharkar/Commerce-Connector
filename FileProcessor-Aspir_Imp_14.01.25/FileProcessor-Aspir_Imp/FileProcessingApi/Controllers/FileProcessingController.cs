using Microsoft.AspNetCore.Http;
using FileProcessingApi.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Commerce_Connector_Data.DataModel;
using Commerce_Connector_Data.CommerceConnectorServices.IService;
using Hangfire;
using LocationConfiguration = FileProcessingApi.Models.LocationConfiguration;
using FileProcessingApi.Mapper;
using Commerce_Connector_Factory.Factories;
using Commerce_Connector_Factory.LocationHandlers;


namespace FileProcessingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileProcessingController : ControllerBase     
    {
        private readonly IBus _bus ;
        private readonly IFileDetailsService _fileDetailsService;
        private readonly IFileLocationHandlerFactory _fileLocationHandlerFactory;
        private readonly IFileProcessingMapper _fileProcessingMapper;

        public FileProcessingController(IBus bus, IFileDetailsService fileDetailsService, IFileLocationHandlerFactory fileLocationHandlerFactory, IFileProcessingMapper fileProcessingMapper)
        {
            _bus = bus;
            _fileDetailsService = fileDetailsService;
            _fileLocationHandlerFactory = fileLocationHandlerFactory;
            _fileProcessingMapper = fileProcessingMapper;

        }

        [HttpPost("CreateRecurringJob")]
        public async Task<IActionResult> CreateRecurringJob([FromQuery] string cronExpression)
        {
            // Fetch the most recent file detail from the database
            var recentFileDetail = await _fileDetailsService.GetMostRecentFileDetailAsync();

            if (recentFileDetail == null)
            {
                return NotFound("No recent file detail found in the database.");
            }

            if (string.IsNullOrWhiteSpace(cronExpression))
            {
                return BadRequest("Cron Expression is required for recurring job.");
            }

            // Scheduling the Recurring Job using Cron Expression
            RecurringJob.AddOrUpdate(
                "process-file-job", // Job identifier
                () => ProcessFile(new FileProcessingMessage
                {
                    JobId = recentFileDetail.JobId,
                    FilePath = recentFileDetail.FilePath,
                    FileType = recentFileDetail.FileType,
                    LocationType = recentFileDetail.LocationType,
                    ModelType = recentFileDetail.ModelType,
                    JobType = "Recurring"
                }),
                cronExpression // User-provided cron expression
            );

            return Ok("Recurring job scheduled.");
        }

        // POST method for one-time job scheduling
        [HttpPost("CreateOneTimeJob")]
        public async Task<IActionResult> CreateOneTimeJob([FromQuery] DateTime jobTime)
        {
            // Fetch the most recent file detail from the database
            var recentFileDetail = await _fileDetailsService.GetMostRecentFileDetailAsync();

            if (recentFileDetail == null)
            {
                return NotFound("No recent file detail found in the database.");
            }

            // Schedule a one-time job to run at the specified time
            BackgroundJob.Schedule(() => ProcessFile(new FileProcessingMessage
            {
                JobId = recentFileDetail.JobId,
                FilePath = recentFileDetail.FilePath,
                FileType = recentFileDetail.FileType,
                LocationType = recentFileDetail.LocationType,
                ModelType = recentFileDetail.ModelType,
                JobType = "OneTime"
            }), jobTime); // Schedule at the user-defined time

            return Ok("One-time job scheduled.");
        }

        [HttpPost("ImmidiateJob")]
        public IActionResult ImmidiateJob(FileProcessingMessage message)
        {
            var fileProcessingMessage = new FileProcessingMessage
            {
                JobId = message.JobId,
                JobType = "ImmidiateJob",
                FileType = message.FileType,
                FilePath = message.FilePath,
                ModelType = message.ModelType,
                LocationConfig = message.LocationConfig,
                LocationType = message.LocationType,
            };

            // Schedule the job for immediate execution
            BackgroundJob.Enqueue(() => ProcessFile(fileProcessingMessage));

            return Ok("Job scheduled successfully.");
        }

        [HttpPost("ProcessFile")]
        public async Task<IActionResult> ProcessFile([FromBody] FileProcessingMessage message)
        {
            if (message == null)
                return BadRequest("Invalid message");

            if (message.JobType == "Recurring" || message.JobType == "OneTime")
            {
                var jobSpecificData = _fileDetailsService.GetJobSpecificData((int)message.JobId);

                // Use mapper for location configuration
                _fileProcessingMapper.MapLocationConfiguration(message, jobSpecificData);

                var templateDetail = _fileDetailsService.GetTemplateById((int)message.JobId);

                // Use mapper for template details
                _fileProcessingMapper.MapTemplateDetails(message, templateDetail);


                var mappedLocationConfig = _fileProcessingMapper.MapToViewModelLocationConfiguration(message.LocationConfig);
                try
                {
                    IFileLocationHandler fileLocation = (IFileLocationHandler)_fileLocationHandlerFactory.CreateFileLocationHandler(message.LocationType, mappedLocationConfig);
                    if (fileLocation != null)
                    {
                        var filestream = await fileLocation.GetFileStreamAsync(message.FilePath);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error creating file location handler: {ex.Message}", ex);
                }
            }
            else
            {
                var templateDetailsList = _fileDetailsService.GetAllTemplateDetails();

                if (templateDetailsList != null && templateDetailsList.Any())
                {
                    var firstTemplateDetail = templateDetailsList.First();

                    _fileProcessingMapper.MapRecentTemplateDetails(message, firstTemplateDetail);
                }

                message.MappingsJson = _fileDetailsService.GetLatestMappingJsonFromDatabase(message.FilePath);
            }

            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    await _bus.Publish(message, cancellationToken: cts.Token);
                }

                return Accepted();
            }
            catch (OperationCanceledException ex)
            {
                return StatusCode(StatusCodes.Status504GatewayTimeout, "Publishing message timed out." + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the message: " + ex.Message);
            }
        }

        [HttpPost("SaveFileDetails")]
        public ActionResult SaveFileDetails([FromBody] FileProcessingMessage message)
        {
            var locationConfig = message.LocationConfig as Models.LocationConfiguration;

            if (message == null)
                return BadRequest("Invalid message");

            // Ensure `LocationConfig` exists in `FileProcessingMessage` and has these properties
            var locationData = new Commerce_Connector_Data.DataModel.LocationConfiguration
            {
                LocationType = message.LocationType,
                ServerAddress = locationConfig.ServerAddress,
                Username = locationConfig.Username,
                Password = locationConfig.Password,
                ApiEndpoint = locationConfig.ApiEndpoint,
                ApiKey = locationConfig.ApiKey,
                ContainerName = locationConfig.ContainerName,
                ConnectionString = locationConfig.ConnectionString
            };

            var fileDetail = new FileDetail
            {
                FilePath = message.FilePath,
                FileType = message.FileType,
                LocationType = message.LocationType, 
                ModelType = message.ModelType,
                MappingsJson = message.MappingsJson,
                LocationConfigId = new Random().Next(1, 100), // Ideally, this ID should be generated by the database
                LocationConfig = locationData
            };

            // Save file details using the service
            _fileDetailsService.SaveFileDetails(fileDetail);

            // Return a success response
            return Ok("File details saved successfully.");
        }

        [HttpPost("SaveTemplateDetails")]
        public ActionResult SaveTemplateDetails([FromBody] TemplateDetails templateDetails)
        {
            

            var templateDetail = new Commerce_Connector_Data.DataModel.TemplateDetail
            {
                ImportTemplateId = templateDetails.ImportTemplateId, // Access nested model properties
                TemplateName = templateDetails.TemplateName,
                ImportHeadId = templateDetails.ImportHeadId,
                FamilyId = templateDetails.FamilyId,
                PromotionTypeId = templateDetails.PromotionTypeId
            };

            // Save template details using the service or repository
            _fileDetailsService.SaveTemplateDetails(templateDetail);


            // Return a success response
            return Ok("File details saved successfully.");
        }

        [HttpGet("GetJobDetails")]
        public ActionResult GetJobDetails()
        {
            if (_fileDetailsService.GetJobList() != null)
            {
                return Ok(_fileDetailsService.GetJobList());
            }
            return NotFound();
        }

        [HttpGet("GetJobById/{id}")]
        public ActionResult GetJobById(int id)
        {
            var job = _fileDetailsService.GetjobById(id);
            return Ok(job);
        }

        [HttpPost("UpdateJob")]
        public void UpdateJob([FromBody] FileProcessingMessage message)
        {
            var existingJob = _fileDetailsService.GetjobById((int)message.JobId);
            if (existingJob != null)
            {
                existingJob.FilePath = message.FilePath;
                existingJob.MappingsJson = message.MappingsJson;
                existingJob.FileType = message.FileType;
                existingJob.ModelType = message.ModelType;
                existingJob.LocationType = message.LocationType;
                _fileDetailsService.UpdateJob(existingJob);
            }
        }

       

    }
}
