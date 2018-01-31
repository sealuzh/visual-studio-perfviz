using System.Collections.Concurrent;
using System.Collections.Generic;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.Filter
{
    interface IFilter
    {
        ConcurrentDictionary<string, ConcurrentDictionary<string, T>> ApplyFilter<T>(ConcurrentDictionary<string, ConcurrentDictionary<string, T>> inDictionary);
    }
}
