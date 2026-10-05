using Commerce_Connector_Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_Data.CommerceConnectorServices.IService
{
    public interface IFileDetailsService
    {
        void SaveFileDetails(FileDetail fileDetails);
        void SaveTemplateDetails(TemplateDetail templateDetail);
        string GetLatestMappingJsonFromDatabase(string filePath);
        Task<FileDetail> GetMostRecentFileDetailAsync();
        List<TemplateDetail> GetAllTemplateDetails();
        List<FileDetail> GetJobSpecificData(int jobId);
        List<TemplateDetail> GetTemplateById(int id);
        List<FileDetail> GetJobList();
        FileDetail GetjobById(int id);
        void UpdateJob(FileDetail fileDetail);


    }
}
