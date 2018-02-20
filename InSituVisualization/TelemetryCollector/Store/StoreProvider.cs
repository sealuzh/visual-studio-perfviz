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

        public Store<ConcreteMethodException> ExceptionStore
        {
            get
            {
                if (_exceptionStore == null)
                {
                    _exceptionStore = new Store<ConcreteMethodException>("VSIX_Exceptions.json");
                    _exceptionStore.Init();
                }
                return _exceptionStore;
            }
        }
            
        public Store<ConcreteMethodTelemetry> TelemetryStore
        {
            get
            {
                if (_telemetryStore == null)
                {
                    _telemetryStore = new Store<ConcreteMethodTelemetry>("VSIX_Exceptions.json");
                    _telemetryStore.Init();
                }
                return _telemetryStore;
            }
        }

        public void Init()
        {
            if (_stores == null)
            {
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
