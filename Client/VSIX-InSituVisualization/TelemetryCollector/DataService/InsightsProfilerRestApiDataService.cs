using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIX_InSituVisualization.TelemetryCollector.DataService
{
    class InsightsProfilerRestApiDataService : IDataService
    {

        Task<IList<ConcreteMemberTelemetry>> IDataService.GetNewTelemetriesTaskAsync()
        {
            throw new NotImplementedException();
        }
    }
}
