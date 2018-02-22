using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.Filter;
using InSituVisualization.TelemetryCollector.Filter.Property;
using InSituVisualization.TelemetryCollector.Persistance;

namespace InSituVisualization.TelemetryCollector.Store
{
    public class Store<T>
    {
        private readonly FilterController<T> _filterController = new FilterController<T>();

        private readonly IPersistentStorage _persistentStorage;

        public Store(IPersistentStorage persistentStorage)
        {
            _persistentStorage = persistentStorage;
        }

        public ConcurrentDictionary<string, ConcurrentDictionary<string, T>> AllMethodTelemetries { get; private set; } = new ConcurrentDictionary<string, ConcurrentDictionary<string, T>>();

        public ConcurrentDictionary<string, ConcurrentDictionary<string, T>> CurrentMethodTelemetries { get; private set; } = new ConcurrentDictionary<string, ConcurrentDictionary<string, T>>();

        public async Task LoadAsync()
        {
            AllMethodTelemetries = await _persistentStorage.GetDataAsync<T>();
            CurrentMethodTelemetries = _filterController.ApplyFilters(AllMethodTelemetries);
        }

        [Conditional("DEBUG")]
        public void AddDebugFilters()
        {
            _filterController.AddFilterGlobal(
                _filterController.GetFilterProperties()[3],
                FilterKind.IsGreaterEqualThen, new DateTime(2018, 1, 15, 12, 45, 00));
        }

        public async Task UpdateAsync()
        {
            await _persistentStorage.StoreDataAsync(AllMethodTelemetries);
            CurrentMethodTelemetries = _filterController.ApplyFilters(AllMethodTelemetries);
        }
    }
}
