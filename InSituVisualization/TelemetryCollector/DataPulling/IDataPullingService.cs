using System.Collections.Generic;
using System.Threading.Tasks;

namespace InSituVisualization.TelemetryCollector.DataPulling
{
    interface IDataPullingService
    {
        Task<IList<ConcreteMethodTelemetry>> GetNewTelemetriesTaskAsync();
    }
}
