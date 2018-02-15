using System.Collections.Generic;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public static class DataCollectionServiceProdvider
    {
        private static List<IDataCollectionService> _dataPullingServices;

        public static List<IDataCollectionService> GetDataCollectionServices()
        {
            return _dataPullingServices ?? (_dataPullingServices =
                       new List<IDataCollectionService> {new InsightsExternalReferencesRestApiDataCollectionService()});
        }        
    }
}
