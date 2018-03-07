using System.Collections.Generic;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    internal interface IFilter
    {
        IEnumerable<RecordedMethodTelemetry> ApplyFilter(IEnumerable<RecordedMethodTelemetry> list);
    }
}
