using System.Collections.Generic;
using InSituVisualization.Model;

namespace InSituVisualization.TelemetryCollector
{
    internal interface ITelemetryProvider
    {
        IDictionary<string, BundleMethodTelemetry> TelemetryData { get; }
    }
}
