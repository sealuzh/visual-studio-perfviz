using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.Model;

namespace InSituVisualization.TelemetryCollector
{
    public interface ITelemetryCollector
    {
        Task<IList<RecordedMethodTelemetry>> GetTelemetryAsync();
    }
}
