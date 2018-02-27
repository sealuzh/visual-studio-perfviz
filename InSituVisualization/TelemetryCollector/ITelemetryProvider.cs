using System.Collections.Generic;
using InSituVisualization.TelemetryCollector.Model;

namespace InSituVisualization.TelemetryCollector
{
    internal interface ITelemetryProvider
    {
        IDictionary<string, AveragedMethod> TelemetryData { get; }
    }
}
