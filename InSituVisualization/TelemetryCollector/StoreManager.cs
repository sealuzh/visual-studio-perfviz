using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.DataCollection;
using InSituVisualization.TelemetryCollector.Model.AveragedMember;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;
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
        private ConcurrentDictionary<string, AveragedMethod> _telemetryData;

        public StoreManager(DataCollectionServiceProvider dataCollectionServiceProvider)
        {
            _dataCollectionServiceProvider = dataCollectionServiceProvider;
        }

        private Store<ConcreteMethodTelemetry> TelemetryStore { get; } = new Store<ConcreteMethodTelemetry>(new FilePersistentStorage(Path.Combine(BasePath, "VSIX_Telemetries.json")));
        private Store<ConcreteMethodException> ExceptionStore { get; } = new Store<ConcreteMethodException>(new FilePersistentStorage(Path.Combine(BasePath, "VSIX_Exceptions.json")));

        public IDictionary<string, AveragedMethod> TelemetryData => _telemetryData;

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
                foreach (var restReturnMember in newRestData)
                {
                    switch (restReturnMember.Dependency.Type)
                    {
                        case "telemetry":
                            var telemetry = restReturnMember.GetConcreteMethodTelemetry();
                            if (TelemetryStore.AllMethodTelemetries
                                .ContainsKey(telemetry.DocumentationCommentId))
                            {
                                {
                                    if (!TelemetryStore.AllMethodTelemetries[telemetry.DocumentationCommentId].ContainsKey(telemetry.Id))
                                    {
                                        //element is missing --> new element. Add it to the dict
                                        if (!TelemetryStore.AllMethodTelemetries[telemetry.DocumentationCommentId]
                                            .TryAdd(telemetry.Id, telemetry))
                                        {
                                            Debug.WriteLine(
                                                "Could not add element " + telemetry.DocumentationCommentId);
                                        }
                                        updateOccured = true;
                                    } //else: already exists, no need to add it
                                }
                            }
                            else
                            {
                                //case methodname does not exist: add a new dict for the new method, put the element inside.
                                var newDict = new ConcurrentDictionary<string, ConcreteMethodTelemetry>();
                                if (!newDict.TryAdd(telemetry.Id, telemetry))
                                {
                                    Debug.WriteLine("Could not add dict " + telemetry.Id);
                                }
                                if (!TelemetryStore.AllMethodTelemetries
                                    .TryAdd(telemetry.DocumentationCommentId, newDict))
                                {
                                    Debug.WriteLine("Could not add dict " + telemetry.DocumentationCommentId);
                                }
                                updateOccured = true;
                            }
                            break;
                        case "exception":
                            var exception = restReturnMember.GetConcreteMethodException();
                            if (ExceptionStore.AllMethodTelemetries
                                .ContainsKey(exception.DocumentationCommentId))
                            {
                                {
                                    if (!ExceptionStore.AllMethodTelemetries[
                                        exception.DocumentationCommentId].ContainsKey(exception.Id))
                                    {
                                        //element is missing --> new element. Add it to the dict
                                        if (!ExceptionStore.AllMethodTelemetries[
                                                exception.DocumentationCommentId]
                                            .TryAdd(exception.Id, exception))
                                        {
                                            Debug.WriteLine("Could not add element " + exception.DocumentationCommentId);
                                        }
                                        updateOccured = true;
                                    } //else: already exists, no need to add it
                                }
                            }
                            else
                            {
                                //case methodname does not exist: add a new dict for the new method, put the element inside.
                                var newDict = new ConcurrentDictionary<string, ConcreteMethodException>();
                                if (!newDict.TryAdd(exception.Id, exception))
                                {
                                    Debug.WriteLine("Could not add dict " + exception.Id);
                                }
                                if (!ExceptionStore.AllMethodTelemetries.TryAdd(exception.DocumentationCommentId, newDict))
                                {
                                    Debug.WriteLine("Could not add dict " + exception.DocumentationCommentId);
                                }
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

        private ConcurrentDictionary<string, AveragedMethod> GenerateAveragedMethodDictionary()
        {
            // TODO RR: Refactor
            var telemetryData = TelemetryStore.CurrentMethodTelemetries;
            var exceptionData = ExceptionStore.CurrentMethodTelemetries;
            //TODO JO: change this, not nice (should not use if)
            var averagedDictionary = new ConcurrentDictionary<string, AveragedMethod>();
            foreach (var key in telemetryData.Keys)
            {
                if (exceptionData.ContainsKey(key))
                {
                    if (!averagedDictionary.TryAdd(key, new AveragedMethod(key, telemetryData[key], exceptionData[key])))
                    {
                        Debug.WriteLine("Could not add element to dictionary with key " + key);
                    }
                }
                else
                {
                    if (!averagedDictionary.TryAdd(key, new AveragedMethod(key, telemetryData[key])))
                    {
                        Debug.WriteLine("Could not add element to dictionary with key " + key);
                    }
                }
            }
            return averagedDictionary;
        }
    }
}
