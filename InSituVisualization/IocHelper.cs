//#define TestMapping
using DryIoc;
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
            Container.Register<InsightsRestApiClient>();
            Container.Register<InsightsRestApiDataMapper>(Reuse.Singleton);
            Container.Register<ITelemetryCollector, InsightsRestApiCollector>();
            Container.Register<CustomSpanProvider>(Reuse.Singleton);
            Container.Register<MemberPerformanceAdorner>(Reuse.Singleton);
#if TestMapping
            Container.Register<ITelemetryDataMapper, MockTelemetryDataMapper>(Reuse.Singleton);
#else
            Container.Register<TelemetryCollectorRegistry>(Reuse.Singleton);
            Container.Register<ITelemetryProvider, TelemetryProvider>(Reuse.Singleton);
            Container.Register<ITelemetryDataMapper, TelemetryDataMapper>(Reuse.Singleton);
#endif
        }
    }
}
