using System.Collections.Generic;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    public class DataCollectionServiceProvider
    {
        private static List<IDataCollectionService> _dataPullingServices;

        public static IEnumerable<IDataCollectionService> GetDataCollectionServices()
        {
            return _dataPullingServices ?? (_dataPullingServices = new List<IDataCollectionService> { new InsightsExternalReferencesRestApiDataCollectionService() });
        }
    }
}
