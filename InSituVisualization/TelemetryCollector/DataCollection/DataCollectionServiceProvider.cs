using System.Collections.Generic;
using DryIoc;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class DataCollectionServiceProvider
    {
        private static List<IDataCollector> _dataPullingServices;

        public IEnumerable<IDataCollector> GetDataCollectionServices()
        {
            return _dataPullingServices ?? (_dataPullingServices = new List<IDataCollector>
            {
                IocHelper.Container.Resolve<IDataCollector>()
            });
        }
    }
}
