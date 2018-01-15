﻿#define TestMapping
using DryIoc;
using VSIX_InSituVisualization.TelemetryCollector;
using VSIX_InSituVisualization.TelemetryMapper;
using VSIX_InSituVisualization.Utils;

namespace VSIX_InSituVisualization
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
            Container.Register<ITelemetryDataProvider, AzureTelemetryStore>(Reuse.Singleton);
            Container.Register<ITelemetryDataMapper, TelemetryDataMapper>(Reuse.Singleton);
#endif
        }
    }
}
