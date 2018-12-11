using DryIoc;
using InSituVisualization.Filter;
using InSituVisualization.Model;
using InSituVisualization.Predictions;
using InSituVisualization.TelemetryCollector;
using InSituVisualization.TelemetryCollector.Insights;
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
            Container.Register<Settings>(Reuse.Singleton);
            Container.Register<IFilterController, FilterController>(Reuse.Singleton);
            Container.Register<ISystemWorkload, SystemWorkload>(Reuse.Singleton);
            // Register more complex predictions here
            Container.Register<IPredictionEngine, LinearPredictionEngine>(Reuse.Singleton);
#if TESTFILTER
            Container.Register<IFilterController, MockFilterController>(Reuse.Singleton, setup: Setup.Decorator);
#endif
            Container.Register<IMethodPerformanceData, MethodPerformanceData>();
            Container.Register<InsightsRestApiClient>();
            Container.Register<InsightsRestApiDataMapper>(Reuse.Singleton);
            Container.Register<ITelemetryCollector, InsightsRestApiCollector>();
            Container.Register<CustomSpanProvider>(Reuse.Singleton);
#if TESTMAPPING
            Container.Register<ITelemetryDataMapper, MockTelemetryDataMapper>(Reuse.Singleton);
#else
            Container.Register<TelemetryCollectorRegistry>(Reuse.Singleton);
            Container.Register<ITelemetryProvider, TelemetryProvider>(Reuse.Singleton);
            Container.Register<ITelemetryDataMapper, TelemetryDataMapper>(Reuse.Singleton);
#endif
        }
    }
}
