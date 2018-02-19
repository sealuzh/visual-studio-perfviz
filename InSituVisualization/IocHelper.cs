﻿//#define TestMapping
using DryIoc;
using InSituVisualization.TelemetryCollector;
using InSituVisualization.TelemetryCollector.DataCollection;
using InSituVisualization.TelemetryCollector.Store;
using InSituVisualization.TelemetryMapper;
using InSituVisualization.Utils;

namespace InSituVisualization
{
    internal static class IocHelper
    {
        static IocHelper()
        {
            Register();
        }

        private static Container _container;

        public static Container Container => _container ?? (_container = new Container());

        public static void Register()
        {
            Container.Register<CustomSpanProvider>();
            Container.Register<MemberPerformanceAdorner>();
#if TestMapping
            Container.Register<ITelemetryDataMapper, MockTelemetryDataMapper>(Reuse.Singleton);
#else
            Container.Register<DataCollectionServiceProvider>(Reuse.Singleton);
            Container.Register<StoreProvider>(Reuse.Singleton);
            Container.Register<ITelemetryProvider, StoreHandler>(Reuse.Singleton);
            Container.Register<ITelemetryDataMapper, TelemetryDataMapper>(Reuse.Singleton);
#endif
        }
    }
}
