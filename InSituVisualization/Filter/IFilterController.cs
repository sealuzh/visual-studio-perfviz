using System;
using System.Collections.Generic;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    public interface IFilterController
    {
        IList<IFilter> Filters { get; }

        event EventHandler FiltersChanged;

        IList<T> ApplyFilters<T>(IList<T> list) where T : RecordedMethodTelemetry;
    }
}