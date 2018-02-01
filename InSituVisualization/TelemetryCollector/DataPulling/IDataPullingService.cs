using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.DataPulling
{
    public interface IDataPullingService
    {
        Task<IList<PulledDataEntity>> GetNewTelemetriesTaskAsync();
    }
}
