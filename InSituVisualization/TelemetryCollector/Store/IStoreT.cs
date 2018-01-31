using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using InSituVisualization.TelemetryCollector.Filter;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;
using InSituVisualization.TelemetryCollector.Persistance;

namespace InSituVisualization.TelemetryCollector.Store
{
    public interface IStoreT<T>
    {
        FilterController<T> GetFilterController();
        PersistanceService<T> GetPersistanceService();
        ConcurrentDictionary<string, ConcurrentDictionary<string, T>> GetAllMethodTelemetries();
        ConcurrentDictionary<string, ConcurrentDictionary<string, T>> GetCurrentMethodTelemetries();
        
    }
}
