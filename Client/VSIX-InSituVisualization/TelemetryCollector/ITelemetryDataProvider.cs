using System.Collections.Generic;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    internal interface ITelemetryDataProvider
    {
        Dictionary<string, AveragedMethodTelemetry> GetAveragedMemberTelemetry();
    }
}
