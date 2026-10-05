using Commerce_Connector_Data.CommerceConnectorServices.IService;
using Commerce_Connector_Data.DataModel;
using Commerce_Connector_Data.FileProcessingEntities;
using Microsoft.EntityFrameworkCore;

namespace Commerce_Connector_Data.CommerceConnectorServices.Service
{
    public class FileDetailsService : IFileDetailsService
    {
        private readonly FileProcessing_Entities _context;

        public FileDetailsService(FileProcessing_Entities context)
        {
            _context = context;
        }

        public void SaveFileDetails(FileDetail fileDetails)
        {
            try
            {
                if (fileDetails.LocationConfig != null)
                {
                    _context.LocationConfigurations.Add(fileDetails.LocationConfig);
                }

                // Save LocationConfig if it exists
                if (fileDetails.LocationConfig != null)
                {
                    _context.SaveChanges(); // Save LocationConfig first
                    fileDetails.LocationConfigId = fileDetails.LocationConfig.Id;
                }

                _context.FileDetail.Add(fileDetails);
                _context.SaveChanges(); // Save FileDetail
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging framework of choice)
                Console.WriteLine($"An error occurred while saving file details: {ex.Message}");
                throw; // Rethrow or handle accordingly
            }
        }

        public void SaveTemplateDetails(TemplateDetail templateDetails)
        {
            if (templateDetails == null)
                throw new ArgumentNullException(nameof(templateDetails));

            // Add the template details to the database
            _context.TemplateDetails.Add(templateDetails);

            // Save changes to persist data
            _context.SaveChanges();
        }

        public string GetLatestMappingJsonFromDatabase(string filePath)
        {
            // Using _context to query the database, assuming 'Id' can be used to determine the latest record
            var mappingJson = _context.FileDetail
                .Where(fd => fd.FilePath == filePath) // Filter based on FilePath
                .OrderByDescending(fd => fd.JobId) // Order by Id to get the most recent entry
                .Select(fd => fd.MappingsJson) // Select the MappingJson column
                .FirstOrDefault(); // Get the first result, or null if not found

            return mappingJson; // This will return either the mapping JSON or null
        }
        public async Task<FileDetail> GetMostRecentFileDetailAsync()
        {
            var fileDetail = await _context.FileDetail
                .OrderByDescending(fd => fd.JobId) 
                .FirstOrDefaultAsync(); 

            return fileDetail;
        }


        public List<TemplateDetail> GetAllTemplateDetails()
        {
            // Fetch all TemplateDetails records from the database and sort by Id in descending order
            var templateDetails = _context.TemplateDetails
                                          .OrderByDescending(t => t.Id) 
                                          .ToList();

            return templateDetails;
        }
        public List<FileDetail> GetJobSpecificData(int jobId)
        {
            var data = _context.FileDetail
                               .Where(j => j.JobId == jobId) 
                               .Include(f => f.LocationConfig) 
                               .ToList(); 

            return data;
        }

        public List<TemplateDetail> GetTemplateById(int jobId)
        {
            var template = _context.TemplateDetails.Where(t => t.Id == jobId).ToList();
            return template;
        }

        public List<FileDetail> GetJobList()
        {
            var jobList = _context.FileDetail
                                  .Include(f => f.LocationConfig)  
                                  .Select(f => new FileDetail
                                  {
                                      JobId = f.JobId, 
                                      FileType = f.FileType,
                                      ModelType = f.ModelType,
                                      LocationType = f.LocationType,
                                      MappingsJson = f.MappingsJson,
                                      LocationConfig = f.LocationConfig
                                  })
                                  .ToList();

            return jobList;
        }
        public FileDetail GetjobById(int id)
        {
            return _context.FileDetail.Find(id);
            
        }

        public void UpdateJob(FileDetail fileDetail)
        {
            _context.FileDetail.Update(fileDetail); 
            _context.SaveChanges();
        }
    }
}
