using System.Collections.Generic;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class DataCollectionServiceProvider
    {
        private readonly Settings _settings;
        private static List<IDataCollector> _dataPullingServices;

        public DataCollectionServiceProvider(Settings settings)
        {
            _settings = settings;
        }

        public IEnumerable<IDataCollector> GetDataCollectionServices()
        {
            return _dataPullingServices ?? (_dataPullingServices = new List<IDataCollector> { new InsightsRestApiDataCollector(_settings.AppId, _settings.ApiKey, _settings.MaxPullingAmount) });
        }
    }
}
