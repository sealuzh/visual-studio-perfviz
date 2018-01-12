using System.Collections.Generic;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter
{
    interface IFilter
    {
        IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> ApplyFilter(IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> inDictionary);
    }
}
