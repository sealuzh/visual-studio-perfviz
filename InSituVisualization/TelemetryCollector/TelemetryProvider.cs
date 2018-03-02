using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
            StartBackgroundTask(CancellationToken.None);
        }

        private HashSet<string> AcknowledgedTelemetryIds { get; } = new HashSet<string>();
        private ConcurrentDictionary<string, MethodPerformanceData> TelemetryData { get; } = new ConcurrentDictionary<string, MethodPerformanceData>();

        public Task<MethodPerformanceData> GetTelemetryDataAsync(string documentationCommentId)
        {
            return TelemetryData.TryGetValue(documentationCommentId, out var methodTelemetry) ? Task.FromResult(methodTelemetry) : Task.FromResult((MethodPerformanceData)null);
        }

        private void StartBackgroundTask(CancellationToken cancellationToken)
        {
            // not awaiting new Task
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await UpdateStoresAsync();
                    }
                    catch (Exception e)
                    {
                        // TODO JO: needed? better catching expected exceptions and letting unexpected ones crash the program... fail fast and hard...
                        Debug.WriteLine(e);
                    }
                    finally
                    {
                        await Task.Delay(TaskDelay, cancellationToken);
                    }
                }
            }, cancellationToken);
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
                            var telemetry = RecordedDurationMethodTelemetry.FromDataEntity(dataEntity);
                            performanceData.Durations.Add(telemetry);
                            break;
                        case "exception":
                            var exception = RecordedExceptionMethodTelemetry.FromDataEntity(dataEntity);
                            performanceData.Exceptions.Add(exception);
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
