using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.Store
{
    public static class StoreProvider
    {
        private static Store<ConcreteMethodException> _exceptionStore;
        private static Store<ConcreteMethodTelemetry> _telemetryStore;
        private static List<Store> _stores;

        public static Store<ConcreteMethodException> GetExceptionStore()
        {
            return _exceptionStore ?? (_exceptionStore = new Store<ConcreteMethodException>("VSIX_Exceptions.json"));
        }

        public static Store<ConcreteMethodTelemetry> GetTelemetryStore()
        {
            return _telemetryStore ?? (_telemetryStore = new Store<ConcreteMethodTelemetry>("VSIX_Telemetries.json"));
        }

        public static List<Store> GetStores()
        {
            if (_stores == null)
            {
                _stores = new List<Store>
                {
                    GetTelemetryStore(),
                    GetExceptionStore()
                };
            }
            return _stores;
        }

        
    }
}
