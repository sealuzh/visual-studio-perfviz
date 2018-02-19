using System.Collections.Generic;
using System.Threading.Tasks;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public interface IDataCollectionService
    {
        Task<IList<CollectedDataEntity>> GetNewTelemetriesTaskAsync();
    }
}
