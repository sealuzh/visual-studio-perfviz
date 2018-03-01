using System.Collections.Generic;
using System.Threading.Tasks;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public interface IDataCollector
    {
        Task<IList<CollectedDataEntity>> GetTelemetryAsync();
    }
}
