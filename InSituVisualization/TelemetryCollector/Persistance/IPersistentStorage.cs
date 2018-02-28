using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace InSituVisualization.TelemetryCollector.Persistance
{
    public interface IPersistentStorage
    {
        Task<ConcurrentDictionary<string, T>> GetDataAsync<T>();
        Task StoreDataAsync<T>(ConcurrentDictionary<string, T> toStoreTelemetryData);
    }
}