using System.Collections.Generic;
using DryIoc;

namespace InSituVisualization.TelemetryCollector
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class TelemetryCollectorRegistry
    {
        public IEnumerable<ITelemetryCollector> TelemetryCollectors { get; } = new List<ITelemetryCollector>
        {
            IocHelper.Container.Resolve<ITelemetryCollector>()
        };
    }
}
