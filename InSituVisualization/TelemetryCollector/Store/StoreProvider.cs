using System;
using System.Collections.Generic;
using System.Diagnostics;
using InSituVisualization.TelemetryCollector.Filter.Property;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.Store
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class StoreProvider
    {
        private Store<ConcreteMethodException> _exceptionStore;
        private Store<ConcreteMethodTelemetry> _telemetryStore;
        private List<Store> _stores;

        public Store<ConcreteMethodException> ExceptionStore =>
            _exceptionStore ?? (_exceptionStore = new Store<ConcreteMethodException>("VSIX_Exceptions.json"));

        public Store<ConcreteMethodTelemetry> TelemetryStore =>
            _telemetryStore ?? (_telemetryStore = new Store<ConcreteMethodTelemetry>("VSIX_Telemetries.json"));

        public void Init()
        {
            if (_stores == null)
            {
                TelemetryStore.Init();
                ExceptionStore.Init();

                _stores = new List<Store>
                {
                    TelemetryStore,
                    ExceptionStore
                };
            }
            //first time build of averagedDictionary
            UpdateStores(false);
        }

        [Conditional("DEBUG")]
        public void AddDebugFilters()
        {
            TelemetryStore.GetFilterController().AddFilterGlobal(
                TelemetryStore.GetFilterController().GetFilterProperties()[3],
                FilterKind.IsGreaterEqualThen, new DateTime(2018, 1, 15, 12, 45, 00));
        }

        public void UpdateStores(bool persist)
        {
            foreach (var store in _stores)
            {
                store.Update(persist);
            }
        }

    }
}
