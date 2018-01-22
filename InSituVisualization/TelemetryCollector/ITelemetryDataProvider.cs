using System.Collections.Generic;

namespace InSituVisualization.TelemetryCollector
{
    internal interface ITelemetryDataProvider
    {
        Dictionary<string, AveragedMethodTelemetry> GetAveragedMemberTelemetry();
    }
}
