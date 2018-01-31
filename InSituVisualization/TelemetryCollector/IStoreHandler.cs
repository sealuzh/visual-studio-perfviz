using System.Collections.Concurrent;
using InSituVisualization.TelemetryCollector.Model.AveragedMember;

namespace InSituVisualization.TelemetryCollector
{
    internal interface IStoreHandler
    {
        ConcurrentDictionary<string, AveragedMethod> GetAveragedMemberTelemetry();
    }
}
