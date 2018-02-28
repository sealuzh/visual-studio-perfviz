//#define TestMapping

using System.IO;
using DryIoc;
using InSituVisualization.TelemetryCollector;
using InSituVisualization.TelemetryCollector.DataCollection;
using InSituVisualization.TelemetryCollector.Persistance;
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
            Container.Register<CustomSpanProvider>(Reuse.Singleton);
            Container.Register<MemberPerformanceAdorner>(Reuse.Singleton);
            Container.Register<IPersistentStorage, FilePersistentStorage>(Reuse.Singleton);
#if TestMapping
            Container.Register<ITelemetryDataMapper, MockTelemetryDataMapper>(Reuse.Singleton);
#else
            Container.Register<DataCollectionServiceProvider>(Reuse.Singleton);
            Container.Register<ITelemetryProvider, StoreManager>(Reuse.Singleton);
            Container.Register<ITelemetryDataMapper, TelemetryDataMapper>(Reuse.Singleton);
#endif
        }
    }
}
