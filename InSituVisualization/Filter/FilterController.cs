using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    // ReSharper disable once ClassNeverInstantiated.Global, Justification: IoC
    internal class FilterController
    {
        public event EventHandler FiltersChanged;

        private readonly ObservableCollection<IFilter> _filters = new ObservableCollection<IFilter>();

        public FilterController()
        {
            _filters.CollectionChanged += (s, e) => FiltersChanged?.Invoke(this, e);
        }

        public IList<IFilter> Filters => _filters;

        //Applies the filters currently stored in _activeFilters.
        public IEnumerable<RecordedMethodTelemetry> ApplyFilters(IEnumerable<RecordedMethodTelemetry> list)
        {
            if (Filters.Count <= 0)
            {
                return list;
            }
            IEnumerable<RecordedMethodTelemetry> result = new List<RecordedMethodTelemetry>(list);
            return Filters.Aggregate(result, (methodTelemetries, filter) => filter.ApplyFilter(methodTelemetries));
        }
    }
}
