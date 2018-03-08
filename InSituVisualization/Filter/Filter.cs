using System.Collections.Generic;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    internal abstract class Filter : IFilter
    {
        protected Filter(FilterKind filterKind)
        {
            FilterKind = filterKind;
        }

        protected FilterKind FilterKind { get; }

        public abstract IList<T> ApplyFilter<T>(IList<T> list) where T : RecordedMethodTelemetry;
    }
}
