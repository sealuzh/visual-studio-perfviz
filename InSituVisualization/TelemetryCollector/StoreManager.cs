﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using InSituVisualization.Model;
using InSituVisualization.TelemetryCollector.DataCollection;
using InSituVisualization.TelemetryCollector.Store;

namespace InSituVisualization.TelemetryCollector
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class StoreManager : ITelemetryProvider
    {
        private readonly DataCollectionServiceProvider _dataCollectionServiceProvider;

        private readonly TimeSpan _taskDelay = TimeSpan.FromMinutes(1);

        private Task _task;
        private readonly ConcurrentDictionary<string, BundleMethodTelemetry> _telemetryData = new ConcurrentDictionary<string, BundleMethodTelemetry>();

        public StoreManager(DataCollectionServiceProvider dataCollectionServiceProvider)
        {
            _dataCollectionServiceProvider = dataCollectionServiceProvider;
        }

        private Store<RecordedDurationMethodTelemetry> TelemetryStore { get; } = new Store<RecordedDurationMethodTelemetry>();
        private Store<RecordedExceptionMethodTelemetry> ExceptionStore { get; } = new Store<RecordedExceptionMethodTelemetry>();

        public Task<BundleMethodTelemetry> GetTelemetryDataAsync(string documentationCommentId)
        {
            // if no information given for this method it does not exist in dict
            if (!_telemetryData.TryGetValue(documentationCommentId, out var methodTelemetry))
            {
                return null;
            }
            return Task.FromResult(methodTelemetry);
        }


        public void StartBackgroundWorker(CancellationToken cancellationToken)
        {
            if (_task != null)
            {
                return;
            }

            UpdateTelemetryData();

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
                var newRestData = await dataCollector.GetNewTelemetriesTaskAsync();
                foreach (var dataEntity in newRestData)
                {
                    switch (dataEntity.DependencyData.Type)
                    {
                        case "telemetry":
                            var telemetry = RecordedDurationMethodTelemetry.FromDataEntity(dataEntity);
                            if (!TelemetryStore.AllMethodTelemetries.ContainsKey(telemetry.Id))
                            {
                                //element is missing --> new element. Add it to the dict
                                TelemetryStore.AllMethodTelemetries.TryAdd(telemetry.Id, telemetry);
                            } //else: already exists, no need to add it
                            break;
                        case "exception":
                            var exception = RecordedExceptionMethodTelemetry.FromDataEntity(dataEntity);
                            if (!ExceptionStore.AllMethodTelemetries.ContainsKey(exception.Id))
                            {
                                //element is missing --> new element. Add it to the dict
                                ExceptionStore.AllMethodTelemetries.TryAdd(exception.Id, exception);
                            } //else: already exists, no need to add it
                            break;
                    }
                }
            }
            UpdateTelemetryData();
        }

        private void UpdateTelemetryData()
        {
            // TODO RR: Remove Durations/Exceptions mapping to reduce this
            foreach (var currentMethodTelemetry in TelemetryStore.CurrentMethodTelemetries.Values)
            {
                if (currentMethodTelemetry.DocumentationCommentId == null)
                {
                    return;
                }
                if (_telemetryData.TryGetValue(currentMethodTelemetry.DocumentationCommentId, out var bundleMethodTelemetry))
                {
                    bundleMethodTelemetry.Durations.Add(currentMethodTelemetry);
                }
                else
                {
                    var bundle = new BundleMethodTelemetry(currentMethodTelemetry.DocumentationCommentId);
                    bundle.Durations.Add(currentMethodTelemetry);
                    _telemetryData.TryAdd(currentMethodTelemetry.DocumentationCommentId, bundle);
                }
            }
            foreach (var currentMethodTelemetry in ExceptionStore.CurrentMethodTelemetries.Values)
            {
                if (currentMethodTelemetry.DocumentationCommentId == null)
                {
                    return;
                }
                if (_telemetryData.TryGetValue(currentMethodTelemetry.DocumentationCommentId, out var bundleMethodTelemetry))
                {
                    bundleMethodTelemetry.Exceptions.Add(currentMethodTelemetry);
                }
                else
                {
                    var bundle = new BundleMethodTelemetry(currentMethodTelemetry.DocumentationCommentId);
                    bundle.Exceptions.Add(currentMethodTelemetry);
                    _telemetryData.TryAdd(currentMethodTelemetry.DocumentationCommentId, bundle);
                }
            }
        }
    }
}
