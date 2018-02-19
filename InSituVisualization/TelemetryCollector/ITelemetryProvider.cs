using System.Collections.Generic;
using InSituVisualization.TelemetryCollector.Model.AveragedMember;

namespace InSituVisualization.TelemetryCollector
{
    internal interface ITelemetryProvider
    {
        IDictionary<string, AveragedMethod> GetAveragedMemberTelemetry();
    }
}
