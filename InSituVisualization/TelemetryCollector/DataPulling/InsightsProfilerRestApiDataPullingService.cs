using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.DataPulling
{
    class InsightsProfilerRestApiDataPullingService : IDataPullingService
    {
        public Task<IList<PulledDataEntity>> GetNewTelemetriesTaskAsync()
        {
            throw new NotImplementedException();
        }

       
    }
}
