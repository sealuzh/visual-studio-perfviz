using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using InSituVisualization.Model;
using InSituVisualization.TelemetryCollector.Filter;
using InSituVisualization.TelemetryCollector.Filter.Property;

namespace InSituVisualization.TelemetryCollector.Store
{
    public class Store<T> where T: RecordedMethodTelemetry
    {
        private readonly FilterController<T> _filterController = new FilterController<T>();
        /// <summary>
        /// Telemetries by ID
        /// </summary>
        public ConcurrentDictionary<string, T> AllMethodTelemetries { get; } = new ConcurrentDictionary<string, T>();

        /// <summary>
        /// Telemetries by ID
        /// </summary>
        public ConcurrentDictionary<string, T> CurrentMethodTelemetries => _filterController.ApplyFilters(AllMethodTelemetries);

        [Conditional("DEBUG")]
        public void AddDebugFilters()
        {
            _filterController.AddFilter(
                _filterController.GetFilterProperties()[3],
                FilterKind.IsGreaterEqualThen, new DateTime(2018, 1, 15, 12, 45, 00));
        }
    }
}
