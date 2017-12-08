using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIX_InSituVisualization.TelemetryCollector.DataPulling
{
    class InsightsProfilerRestApiDataPullingService : IDataPullingService
    {

        Task<IList<ConcreteTelemetryMember>> IDataPullingService.GetNewTelemetriesTaskAsync()
        {
            throw new NotImplementedException();
            
            
        }
    }
}
