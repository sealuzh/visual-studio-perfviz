using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using InSituVisualization.TelemetryCollector.Filter;
using InSituVisualization.TelemetryCollector.Model.AveragedMember;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;
using InSituVisualization.TelemetryCollector.Persistance;

namespace InSituVisualization.TelemetryCollector.Store
{
    public abstract class Store
    {
        public abstract void Update(bool persist);
    }

    public class Store<T> : Store
    {
        protected ConcurrentDictionary<string, ConcurrentDictionary<string, T>> AllMemberTelemetries;
        protected ConcurrentDictionary<string, ConcurrentDictionary<string, T>> CurrentMemberTelemetries;

        protected FilterController<T> FilterController;
        protected readonly PersistanceService<T> PersistanceService;

        public Store(string fileName)
        {
            PersistanceService = new PersistanceService<T>(fileName);

            FilterController = new FilterController<T>();
            AllMemberTelemetries = new ConcurrentDictionary<string, ConcurrentDictionary<string, T>>();
            AllMemberTelemetries = PersistanceService.FetchSystemCacheData();
            CurrentMemberTelemetries = new ConcurrentDictionary<string, ConcurrentDictionary<string, T>>();
        }

        public ConcurrentDictionary<string, ConcurrentDictionary<string, T>> GetAllMethodTelemetries() =>
            AllMemberTelemetries;

        public ConcurrentDictionary<string, ConcurrentDictionary<string, T>> GetCurrentMethodTelemetries() =>
            CurrentMemberTelemetries;

        public PersistanceService<T> GetPersistanceService() => PersistanceService;

        public FilterController<T> GetFilterController() => FilterController;

        public override void Update(bool persist)
        {
            if (persist) PersistanceService.WriteSystemCacheData(AllMemberTelemetries);
            CurrentMemberTelemetries = FilterController.ApplyFilters(AllMemberTelemetries);
        }
    }
}
