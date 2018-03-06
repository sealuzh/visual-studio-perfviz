using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.Model;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public interface IDataCollector
    {
        Task<IList<RecordedMethodTelemetry>> GetTelemetryAsync();
    }
}
