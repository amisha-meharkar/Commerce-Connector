using Commerce_Connector_Data.CommerceConnectorServices.IService;
using Commerce_Connector_Data.DataModel;
using Commerce_Connector_Data.FileProcessingEntities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // Query to get the most recent FileDetail without using Include for non-navigation properties
            var fileDetail = await _context.FileDetail
                .OrderByDescending(fd => fd.JobId) // Ordering by JobId (assuming it's for sorting)
                .FirstOrDefaultAsync(); // Fetching the most recent FileDetail

            return fileDetail;
        }

        //public List<TemplateDetail> GetAllTemplateDetails()
        //{
        //    // Fetch all TemplateDetails records from the database
        //    var templateDetails = _context.TemplateDetails.ToList();

        //    return templateDetails;
        //}

        public List<TemplateDetail> GetAllTemplateDetails()
        {
            // Fetch all TemplateDetails records from the database and sort by Id in descending order
            var templateDetails = _context.TemplateDetails
                                          .OrderByDescending(t => t.Id) // Sort by Id (newest first)
                                          .ToList();

            return templateDetails;
        }
        public List<FileDetail> GetJobSpecificData(int jobId)
        {
            // Eager load FileDetail along with the related LocationConfiguration
            var data = _context.FileDetail
                               .Where(j => j.JobId == jobId) // Filter by JobId
                               .Include(f => f.LocationConfig) // Include the LocationConfig navigation property
                               .ToList(); // Execute the query and return the result

            return data;
        }

        public List<TemplateDetail> GetTemplateById(int jobId)
        {
            // Logic to fetch template details by templateId
            var template = _context.TemplateDetails.Where(t => t.Id == jobId).ToList();
            return template;
        }

        //public List<FileDetail> GetJobList()
        //{
        //    List<FileDetail> products = _context.FileDetail
        //                            .Include(f => f.LocationConfig)
        //                            .AsNoTracking()
        //                            .ToList();

        //    return products;

        //}
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
            _context.FileDetail.Update(fileDetail); // Automatically sets the state to Modified
            _context.SaveChanges();
        }
    }
}
