using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.DataPulling;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;
using InSituVisualization.TelemetryCollector.Store;

namespace InSituVisualization.TelemetryCollector
{
    public static class StoreHandlerSettingsProvider
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
