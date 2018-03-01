using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using InSituVisualization.Model;
using InSituVisualization.TelemetryCollector.DataCollection;

namespace InSituVisualization.TelemetryCollector
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class StoreManager : ITelemetryProvider
    {

        // TODO RR: Filters
        //private readonly FilterController<T> _filterController = new FilterController<T>();

        //public void AddDebugFilters()
        //{
        //    _filterController.AddFilter(
        //        _filterController.GetFilterProperties()[3],
        //        FilterKind.IsGreaterEqualThen, new DateTime(2018, 1, 15, 12, 45, 00));
        //}
        //        public ConcurrentDictionary<string, T> CurrentMethodTelemetries => _filterController.ApplyFilters(AllMethodTelemetries);



        private readonly DataCollectionServiceProvider _dataCollectionServiceProvider;

        private readonly TimeSpan _taskDelay = TimeSpan.FromMinutes(1);

        private Task _task;
        private readonly ConcurrentDictionary<string, MethodPerformanceData> _telemetryData = new ConcurrentDictionary<string, MethodPerformanceData>();

        public StoreManager(DataCollectionServiceProvider dataCollectionServiceProvider)
        {
            _dataCollectionServiceProvider = dataCollectionServiceProvider;
            StartBackgroundWorker(CancellationToken.None);
        }

        private ConcurrentDictionary<string, RecordedDurationMethodTelemetry> TelemetryStore { get; } = new ConcurrentDictionary<string, RecordedDurationMethodTelemetry>();
        private ConcurrentDictionary<string, RecordedExceptionMethodTelemetry> ExceptionStore { get; } = new ConcurrentDictionary<string, RecordedExceptionMethodTelemetry>();

        public Task<MethodPerformanceData> GetTelemetryDataAsync(string documentationCommentId)
        {
            return _telemetryData.TryGetValue(documentationCommentId, out var methodTelemetry) ? Task.FromResult(methodTelemetry) : Task.FromResult((MethodPerformanceData)null);
        }


        private void StartBackgroundWorker(CancellationToken cancellationToken)
        {
            if (_task != null)
            {
                return;
            }
            // not awaiting new Task
            _task = Task.Run(async () =>
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
                        await Task.Delay(_taskDelay, cancellationToken);
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
                    if (string.IsNullOrWhiteSpace(dataEntity.DependencyData.Name))
                    {
                        continue;
                    }
                    switch (dataEntity.DependencyData.Type)
                    {
                        case "telemetry":
                            var telemetry = RecordedDurationMethodTelemetry.FromDataEntity(dataEntity);
                            if (!TelemetryStore.ContainsKey(telemetry.Id))
                            {
                                //element is missing --> new element. Add it to the dict
                                TelemetryStore.TryAdd(telemetry.Id, telemetry);

                                if (_telemetryData.TryGetValue(telemetry.DocumentationCommentId, out var methodPerformanceData))
                                {
                                    methodPerformanceData.Durations.Add(telemetry);
                                }
                                else
                                {
                                    var data = new MethodPerformanceData();
                                    data.Durations.Add(telemetry);
                                    _telemetryData.TryAdd(telemetry.DocumentationCommentId, data);
                                }
                            }
                            break;
                        case "exception":
                            var exception = RecordedExceptionMethodTelemetry.FromDataEntity(dataEntity);
                            if (!ExceptionStore.ContainsKey(exception.Id))
                            {
                                //element is missing --> new element. Add it to the dict
                                ExceptionStore.TryAdd(exception.Id, exception);

                                if (_telemetryData.TryGetValue(exception.DocumentationCommentId, out var methodPerformanceData))
                                {
                                    methodPerformanceData.Exceptions.Add(exception);
                                }
                                else
                                {
                                    var data = new MethodPerformanceData();
                                    data.Exceptions.Add(exception);
                                    _telemetryData.TryAdd(exception.DocumentationCommentId, data);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
