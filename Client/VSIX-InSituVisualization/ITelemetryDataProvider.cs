using System.Collections.Generic;
using VSIX_InSituVisualization.TelemetryCollector;

namespace VSIX_InSituVisualization
{
    internal interface ITelemetryDataProvider
    {
        Dictionary<string, AveragedTelemetry> GetAveragedMemberTelemetry();
    }
}
