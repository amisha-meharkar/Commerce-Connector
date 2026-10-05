using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Models;

namespace Commerce_Connector_BAL.IAgent
{
    public interface ITemplateAgent
    {
        Task<List<ImportTemplateModel>> GetTemplatesAsync(int importHeadId, int familyId, int promotionTypeId = 0);
    }
}
