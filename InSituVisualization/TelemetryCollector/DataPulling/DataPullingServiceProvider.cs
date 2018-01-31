using System.Collections.Generic;

namespace InSituVisualization.TelemetryCollector.DataPulling
{
    public static class DataPullingServiceProdvider
    {
        private static List<IDataPullingService> _dataPullingServices;

        public static List<IDataPullingService> GetDataPullingServices()
        {
            if (_dataPullingServices == null)
            {
                _dataPullingServices = new List<IDataPullingService> {new InsightsExternalReferencesRestApiDataPullingService()};
            }
            return _dataPullingServices;
        }        
    }
}
