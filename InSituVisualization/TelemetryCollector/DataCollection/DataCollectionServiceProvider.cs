using System.Collections.Generic;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    public class DataCollectionServiceProvider
    {
        private static List<IDataCollector> _dataPullingServices;

        public static IEnumerable<IDataCollector> GetDataCollectionServices()
        {
            return _dataPullingServices ?? (_dataPullingServices = new List<IDataCollector> { new InsightsRestApiDataCollector() });
        }
    }
}
