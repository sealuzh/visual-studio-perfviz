using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    // ReSharper disable once ClassNeverInstantiated.Global, Justification: IoC
    public class FilterController : IFilterController
    {
        public event EventHandler FiltersChanged;

        private readonly ObservableCollection<IFilter> _filters = new ObservableCollection<IFilter>();

        public FilterController()
        {
            _filters.CollectionChanged += (s, e) => FiltersChanged?.Invoke(this, e);
        }

        public IList<IFilter> Filters => _filters;

        //Applies the filters currently stored in _activeFilters.
        public IList<T> ApplyFilters<T>(IList<T> list) where T: RecordedMethodTelemetry
        {
            if (!Filters.Any())
            {
                return list;
            }
            return Filters.Aggregate(list, (methodTelemetries, filter) => filter.ApplyFilter(methodTelemetries));
        }
    }
}
