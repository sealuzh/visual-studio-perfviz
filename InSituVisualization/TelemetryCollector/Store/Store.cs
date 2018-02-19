using System.Collections.Concurrent;
using InSituVisualization.TelemetryCollector.Filter;
using InSituVisualization.TelemetryCollector.Persistance;

namespace InSituVisualization.TelemetryCollector.Store
{
    public abstract class Store
    {
        public abstract void Update(bool persist);
    }

    public class Store<T> : Store
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, T>> _allMemberTelemetries = new ConcurrentDictionary<string, ConcurrentDictionary<string, T>>();
        private ConcurrentDictionary<string, ConcurrentDictionary<string, T>> _currentMemberTelemetries = new ConcurrentDictionary<string, ConcurrentDictionary<string, T>>();

        private readonly FilterController<T> _filterController = new FilterController<T>();

        private readonly PersistanceService<T> _persistanceService;

        public Store(string fileName)
        {
            _persistanceService = new PersistanceService<T>(fileName);
        }

        public Store<T> Init()
        {
            _allMemberTelemetries = _persistanceService.FetchSystemCacheData();
            return this;
        }

        public ConcurrentDictionary<string, ConcurrentDictionary<string, T>> GetAllMethodTelemetries() => _allMemberTelemetries;

        public ConcurrentDictionary<string, ConcurrentDictionary<string, T>> GetCurrentMethodTelemetries() => _currentMemberTelemetries;

        public PersistanceService<T> GetPersistanceService() => _persistanceService;

        public FilterController<T> GetFilterController() => _filterController;

        public override void Update(bool persist)
        {
            if (persist)
            {
                _persistanceService.WriteSystemCacheData(_allMemberTelemetries);
            }
            _currentMemberTelemetries = _filterController.ApplyFilters(_allMemberTelemetries);
        }
    }
}
