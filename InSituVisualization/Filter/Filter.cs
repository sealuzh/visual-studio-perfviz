using System.Collections.Generic;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    internal abstract class Filter : IFilter
    {
        public FilterKind FilterKind { get; set; } = FilterKind.None;

        public abstract IList<T> ApplyFilter<T>(IList<T> list) where T : RecordedMethodTelemetry;
    }
}
