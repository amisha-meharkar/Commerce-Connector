using Commerce_Connector_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_BAL.IAgent
{
    public interface IJobsAgent
    {
        Task<HttpResponseMessage> ImmediateJobAsync(FileProcessingMessageViewModel fileMessage);
        Task<HttpResponseMessage> ScheduleOneTimeJobAsync(DateTime jobTime);
        Task<HttpResponseMessage> ScheduleRecurringJobAsync(string cronExpression);
    }
}
