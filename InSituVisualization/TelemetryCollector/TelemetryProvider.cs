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
        private static readonly TimeSpan TaskDelay = TimeSpan.FromMinutes(1);
        private readonly DataCollectionServiceProvider _dataCollectionServiceProvider;

        public TelemetryProvider(DataCollectionServiceProvider dataCollectionServiceProvider)
        {
            _dataCollectionServiceProvider = dataCollectionServiceProvider;

            // Starting background thread
            Task.Run(async () =>
            {
                while (true)
                {
                    await UpdateStoresAsync();
                    await Task.Delay(TaskDelay);
                }
                // ReSharper disable once FunctionNeverReturns
            });
        }

        private HashSet<string> AcknowledgedTelemetryIds { get; } = new HashSet<string>();
        private ConcurrentDictionary<string, MethodPerformanceData> TelemetryData { get; } = new ConcurrentDictionary<string, MethodPerformanceData>();

        public Task<MethodPerformanceData> GetTelemetryDataAsync(string documentationCommentId)
        {
            return TelemetryData.TryGetValue(documentationCommentId, out var methodTelemetry) ? Task.FromResult(methodTelemetry) : Task.FromResult((MethodPerformanceData)null);
        }

        private async Task UpdateStoresAsync()
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

                    TelemetryData.TryGetValue(dataEntity.DependencyData.Name, out var performanceData);
                    if (performanceData == null)
                    {
                        performanceData = new MethodPerformanceData();
                        TelemetryData.TryAdd(dataEntity.DependencyData.Name, performanceData);
                    }

                    switch (dataEntity.DependencyData.Type)
                    {
                        case "telemetry":
                            performanceData.Durations.Add(RecordedDurationMethodTelemetry.FromDataEntity(dataEntity));
                            break;
                        case "exception":
                            performanceData.Exceptions.Add(RecordedExceptionMethodTelemetry.FromDataEntity(dataEntity));
                            break;
                    }
                }
            }
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
