using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.Model;
using InSituVisualization.TelemetryCollector.DataCollection;

namespace InSituVisualization.TelemetryCollector
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class TelemetryProvider : ITelemetryProvider
    {
        private static readonly TimeSpan ThrottleTimeSpan = TimeSpan.FromMinutes(1);
        private readonly DataCollectionServiceProvider _dataCollectionServiceProvider;

        private DateTime _lastUpdate = default(DateTime);

        public TelemetryProvider(DataCollectionServiceProvider dataCollectionServiceProvider)
        {
            _dataCollectionServiceProvider = dataCollectionServiceProvider;
        }

        private HashSet<string> AcknowledgedTelemetryIds { get; } = new HashSet<string>();

        private ConcurrentDictionary<string, MethodPerformanceData> TelemetryByDocumentationCommentId { get; } = new ConcurrentDictionary<string, MethodPerformanceData>();

        public async Task<MethodPerformanceData> GetTelemetryDataAsync(string documentationCommentId)
        {
            if (_lastUpdate + ThrottleTimeSpan < DateTime.Now)
            {
                await UpdateTelemetryDataAsync();
            }
            return TelemetryByDocumentationCommentId.TryGetValue(documentationCommentId, out var methodTelemetry) ? methodTelemetry : null;
        }

        private async Task UpdateTelemetryDataAsync()
        {
            foreach (var dataCollector in _dataCollectionServiceProvider.GetDataCollectionServices())
            {
                var newRestData = await dataCollector.GetTelemetryAsync();
                foreach (var dataEntity in newRestData)
                {
                    if (!AcknowledgedTelemetryIds.Add(dataEntity.Id) || string.IsNullOrWhiteSpace(dataEntity.DependencyData.Name))
                    {
                        // Telemetry is already known or no documentationCommentId
                        continue;
                    }

                    TelemetryByDocumentationCommentId.TryGetValue(dataEntity.DependencyData.Name, out var performanceData);
                    if (performanceData == null)
                    {
                        performanceData = new MethodPerformanceData();
                        TelemetryByDocumentationCommentId.TryAdd(dataEntity.DependencyData.Name, performanceData);
                    }

                    switch (dataEntity.DependencyData.Type)
                    {
                        case "telemetry":
                            performanceData.ExecutionTimes.Add(RecordedDurationMethodTelemetry.FromDataEntity(dataEntity));
                            break;
                        case "exception":
                            performanceData.Exceptions.Add(RecordedExceptionMethodTelemetry.FromDataEntity(dataEntity));
                            break;
                    }
                }
            }
            _lastUpdate = DateTime.Now;
        }



        // TODO RR: Filters
        //private readonly FilterController<T> _filterController = new FilterController<T>();

        //public void AddDebugFilters()
        //{
        //    _filterController.AddFilter(
        //        _filterController.GetFilterProperties()[3],
        //        FilterKind.IsGreaterEqualThen, new DateTime(2018, 1, 15, 12, 45, 00));
        //}
        //        public ConcurrentDictionary<string, T> CurrentMethodTelemetries => _filterController.ApplyFilters(AllMethodTelemetries);



    }
}
