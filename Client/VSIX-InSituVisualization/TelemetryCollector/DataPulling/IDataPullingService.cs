using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIX_InSituVisualization.TelemetryCollector.DataPulling
{
    interface IDataPullingService
    {
        Task<IList<ConcreteMethodTelemetry>> GetNewTelemetriesTaskAsync();
    }
}
