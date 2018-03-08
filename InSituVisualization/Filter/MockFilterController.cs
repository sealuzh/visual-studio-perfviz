using System;
using System.Collections.Generic;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    /// <summary>
    /// Decorator for FilterController
    /// 
    /// Adding Mock Filter
    /// </summary>
    class MockFilterController : IFilterController
    {
        private readonly IFilterController _filterController;

        public MockFilterController(IFilterController filterController)
        {
            _filterController = filterController;

            // Adding Mock Filter
            _filterController.Filters.Add(new ComparableFilter<DateTime>(FilterKind.IsGreaterEqualThen, telemetry => telemetry.Timestamp, new DateTime(2018, 2, 22, 17, 20, 40)));

            _filterController.FiltersChanged += FiltersChanged;
        }

        public IList<IFilter> Filters => _filterController.Filters;

        public event EventHandler FiltersChanged;

        public IList<T> ApplyFilters<T>(IList<T> list) where T : RecordedMethodTelemetry
        {
            return _filterController.ApplyFilters(list);
        }
    }
}
