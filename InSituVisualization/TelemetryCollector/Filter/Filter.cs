using System;
using System.Collections.Concurrent;
using InSituVisualization.TelemetryCollector.Filter.Property;

namespace InSituVisualization.TelemetryCollector.Filter
{
    public abstract class Filter
    {
        protected readonly FilterKind FilterKind;

        protected Filter(FilterKind filterKind)
        {
            FilterKind = filterKind;
        }

        public abstract ConcurrentDictionary<string, T> ApplyFilter<T>(ConcurrentDictionary<string, T> inDictionary);
    }
}
