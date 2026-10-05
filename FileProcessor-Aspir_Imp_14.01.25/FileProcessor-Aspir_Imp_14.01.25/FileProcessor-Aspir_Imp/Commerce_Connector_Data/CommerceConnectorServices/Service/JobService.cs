//using Hangfire;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Commerce_Connector_Data.CommerceConnectorServices.Service
//{
//    public class JobService
//    {
//        private readonly IBackgroundJobClient _backgroundJobClient;

//        public JobService(IBackgroundJobClient backgroundJobClient)
//        {
//            _backgroundJobClient = backgroundJobClient;
//        }

//        public void ScheduleJobs()
//        {
//            RecurringJob.AddOrUpdate<FileProcessingController>(
//                "process-file-job",
//                controller => controller.Index(new FileProcessingMessage
//                {
//                    FilePath = "Sample\\Inventory.csv",
//                    FileType = "CSV",
//                    LocationType = "Local",
//                    ModelType = "Inventory",
//                    MappingsJson = "[{\"MappingId\":0,\"MapCsvColumn\":\"WarehouseCode\",\"MapTargetColumn\":\"WarehouseName\",\"TransitionExpression\":\"\"},{\"MappingId\":1,\"MapCsvColumn\":\"SKU\",\"MapTargetColumn\":\"SKU\",\"TransitionExpression\":\"\"},{\"MappingId\":2,\"MapCsvColumn\":\"Quantity\",\"MapTargetColumn\":\"Quantity\",\"TransitionExpression\":\"[WarehouseCode] + [ReOrderLevel]\"},{\"MappingId\":3,\"MapCsvColumn\":\"ReOrderLevel\",\"MapTargetColumn\":\"ReOrderLevel\",\"TransitionExpression\":\"\"}]"
//                }),
//                "*/5 * * * *"
//            );
//        }
//    }

//}
