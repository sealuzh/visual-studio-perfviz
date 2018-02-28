using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using InSituVisualization.Model;
using InSituVisualization.TelemetryCollector.DataCollection;
using InSituVisualization.TelemetryCollector.Persistance;
using InSituVisualization.TelemetryCollector.Store;

namespace InSituVisualization.TelemetryCollector
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class StoreManager : ITelemetryProvider
    {
        private readonly DataCollectionServiceProvider _dataCollectionServiceProvider;
        private static readonly string BasePath = Path.GetDirectoryName(Path.GetTempPath()) + "\\InSitu";

        private readonly TimeSpan _taskDelay = TimeSpan.FromMinutes(1);

        private Task _task;
        private ConcurrentDictionary<string, BundleMethodTelemetry> _telemetryData;

        public StoreManager(DataCollectionServiceProvider dataCollectionServiceProvider)
        {
            _dataCollectionServiceProvider = dataCollectionServiceProvider;
        }

        private Store<RecordedDurationMethodTelemetry> TelemetryStore { get; } = new Store<RecordedDurationMethodTelemetry>(new FilePersistentStorage(Path.Combine(BasePath, "VSIX_Telemetries.json")));
        private Store<RecordedExceptionMethodTelemetry> ExceptionStore { get; } = new Store<RecordedExceptionMethodTelemetry>(new FilePersistentStorage(Path.Combine(BasePath, "VSIX_Exceptions.json")));

        public IDictionary<string, BundleMethodTelemetry> TelemetryData => _telemetryData;

        public async Task StartBackgroundWorkerAsync(CancellationToken cancellationToken)
        {
            if (_task != null)
            {
                return;
            }

            await TelemetryStore.LoadAsync();
            await ExceptionStore.LoadAsync();
            _telemetryData = GenerateAveragedMethodDictionary();

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
            var updateOccured = false;
            foreach (IDataCollector service in _dataCollectionServiceProvider.GetDataCollectionServices())
            {
                var newRestData = await service.GetNewTelemetriesTaskAsync();
                foreach (var dataEntity in newRestData)
                {
                    switch (dataEntity.DependencyData.Type)
                    {
                        case "telemetry":
                            var telemetry = RecordedDurationMethodTelemetry.FromDataEntity(dataEntity);
                            if (TelemetryStore.AllMethodTelemetries.ContainsKey(telemetry.DocumentationCommentId))
                            {
                                if (!TelemetryStore.AllMethodTelemetries[telemetry.DocumentationCommentId].ContainsKey(telemetry.Id))
                                {
                                    //element is missing --> new element. Add it to the dict
                                    TelemetryStore.AllMethodTelemetries[telemetry.DocumentationCommentId].TryAdd(telemetry.Id, telemetry);
                                    updateOccured = true;
                                } //else: already exists, no need to add it
                            }
                            else
                            {
                                //case methodname does not exist: add a new dict for the new method, put the element inside.
                                var newDict = new ConcurrentDictionary<string, RecordedDurationMethodTelemetry>();
                                newDict.TryAdd(telemetry.Id, telemetry);
                                TelemetryStore.AllMethodTelemetries.TryAdd(telemetry.DocumentationCommentId, newDict);
                                updateOccured = true;
                            }
                            break;
                        case "exception":
                            var exception = RecordedExceptionMethodTelemetry.FromDataEntity(dataEntity);
                            if (ExceptionStore.AllMethodTelemetries.ContainsKey(exception.DocumentationCommentId))
                            {
                                {
                                    if (!ExceptionStore.AllMethodTelemetries[exception.DocumentationCommentId].ContainsKey(exception.Id))
                                    {
                                        //element is missing --> new element. Add it to the dict
                                        ExceptionStore.AllMethodTelemetries[exception.DocumentationCommentId].TryAdd(exception.Id, exception);
                                        updateOccured = true;
                                    } //else: already exists, no need to add it
                                }
                            }
                            else
                            {
                                //case methodname does not exist: add a new dict for the new method, put the element inside.
                                var newDict = new ConcurrentDictionary<string, RecordedExceptionMethodTelemetry>();
                                newDict.TryAdd(exception.Id, exception);
                                ExceptionStore.AllMethodTelemetries.TryAdd(exception.DocumentationCommentId, newDict);
                                updateOccured = true;
                            }
                            break;
                    }
                }
            }
            if (updateOccured)
            {
                await TelemetryStore.UpdateAsync();
                await ExceptionStore.UpdateAsync();
                _telemetryData = GenerateAveragedMethodDictionary();
            }
        }

        private ConcurrentDictionary<string, BundleMethodTelemetry> GenerateAveragedMethodDictionary()
        {
            var telemetryData = TelemetryStore.CurrentMethodTelemetries;
            var exceptionData = ExceptionStore.CurrentMethodTelemetries;
            //TODO JO: change this, not nice (should not use if)
            var averagedDictionary = new ConcurrentDictionary<string, BundleMethodTelemetry>();
            foreach (var key in telemetryData.Keys)
            {
                if (exceptionData.ContainsKey(key))
                {
                    averagedDictionary.TryAdd(key, new BundleMethodTelemetry(key, telemetryData[key], exceptionData[key]));
                }
                else
                {
                    averagedDictionary.TryAdd(key, new BundleMethodTelemetry(key, telemetryData[key], null));
                }
            }
            return averagedDictionary;
        }
    }
}
