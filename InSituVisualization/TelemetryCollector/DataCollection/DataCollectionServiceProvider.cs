using System.Collections.Generic;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public static class DataPullingServiceProdvider
    {
        private static List<IDataPullingService> _dataPullingServices;

        public static List<IDataPullingService> GetDataPullingServices()
        {
            return _dataPullingServices ?? (_dataPullingServices =
                       new List<IDataPullingService> {new InsightsExternalReferencesRestApiDataPullingService()});
        }        
    }
}
