using System.Collections.Generic;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    interface IFilter
    {
        IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> ApplyFilter(IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> inDictionary);
    }
}
