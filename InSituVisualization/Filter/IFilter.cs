using System.Collections.Generic;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    public interface IFilter
    {
        IList<T> ApplyFilter<T>(IList<T> list) where T : RecordedMethodTelemetry;
    }
}
