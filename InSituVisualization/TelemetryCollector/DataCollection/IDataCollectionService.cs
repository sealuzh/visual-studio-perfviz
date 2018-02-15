using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public interface IDataPullingService
    {
        Task<IList<PulledDataEntity>> GetNewTelemetriesTaskAsync();
    }
}
