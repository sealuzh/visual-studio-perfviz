using System.Collections.Concurrent;
using System.Collections.Generic;
using InSituVisualization.TelemetryCollector.Model.AveragedMember;

namespace InSituVisualization.TelemetryCollector
{
    internal interface ITelemetryDataProvider
    {
        ConcurrentDictionary<string, AveragedMethod> GetAveragedMemberTelemetry();
    }
}
