using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.DataPulling
{
    class InsightsProfilerRestApiDataPullingService : IDataPullingService
    {

        Task<IList<ConcreteMethodTelemetry>> IDataPullingService.GetNewTelemetriesTaskAsync()
        {
            throw new NotImplementedException();
            
            
        }
    }
}
