using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.DataCollection;
using InSituVisualization.TelemetryCollector.Model.AveragedMember;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;
using InSituVisualization.TelemetryCollector.Store;

namespace InSituVisualization.TelemetryCollector
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class StoreHandler : ITelemetryProvider
    {
        private readonly TimeSpan _taskDelay = TimeSpan.FromSeconds(5);

        private readonly StoreProvider _storeProvider;

        private ConcurrentDictionary<string, AveragedMethod> _currentAveragedMemberTelemetry;
        private Task _task;

        public StoreHandler(StoreProvider storeProvider)
        {
            _storeProvider = storeProvider;
        }

        public void StartBackgroundTask(CancellationToken cancellationToken)
        {
            _storeProvider.Init();
            _currentAveragedMemberTelemetry = GenerateAveragedMethodDictionary(_storeProvider.TelemetryStore.GetCurrentMethodTelemetries(), _storeProvider.ExceptionStore.GetCurrentMethodTelemetries());

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
            foreach (IDataCollectionService service in DataCollectionServiceProvider.GetDataCollectionServices())
            {
                var newRestData = await service.GetNewTelemetriesTaskAsync();
                foreach (var restReturnMember in newRestData)
                {
                    switch (restReturnMember.Dependency.Type)
                    {
                        case "telemetry":
                            var telemetry = restReturnMember.GetConcreteMethodTelemetry();
                            if (_storeProvider.TelemetryStore.GetAllMethodTelemetries()
                                .ContainsKey(telemetry.DocumentationCommentId))
                            {
                                {
                                    if (!_storeProvider.TelemetryStore.GetAllMethodTelemetries()[telemetry.DocumentationCommentId].ContainsKey(telemetry.Id))
                                    {
                                        //element is missing --> new element. Add it to the dict
                                        if (!_storeProvider.TelemetryStore.GetAllMethodTelemetries()[telemetry.DocumentationCommentId]
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
                                if (!_storeProvider.TelemetryStore.GetAllMethodTelemetries()
                                    .TryAdd(telemetry.DocumentationCommentId, newDict))
                                {
                                    Debug.WriteLine("Could not add dict " + telemetry.DocumentationCommentId);
                                }
                                updateOccured = true;
                            }
                            break;
                        case "exception":
                            var exception = restReturnMember.GetConcreteMethodException();
                            if (_storeProvider.ExceptionStore.GetAllMethodTelemetries()
                                .ContainsKey(exception.DocumentationCommentId))
                            {
                                {
                                    if (!_storeProvider.ExceptionStore.GetAllMethodTelemetries()[
                                        exception.DocumentationCommentId].ContainsKey(exception.Id))
                                    {
                                        //element is missing --> new element. Add it to the dict
                                        if (!_storeProvider.ExceptionStore.GetAllMethodTelemetries()[
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
                                if (!_storeProvider.ExceptionStore.GetAllMethodTelemetries()
                                    .TryAdd(exception.DocumentationCommentId, newDict))
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
                _storeProvider.UpdateStores(true);
                _currentAveragedMemberTelemetry = GenerateAveragedMethodDictionary(
                    _storeProvider.TelemetryStore.GetCurrentMethodTelemetries(),
                    _storeProvider.ExceptionStore.GetCurrentMethodTelemetries());
            }
        }

        private ConcurrentDictionary<string, AveragedMethod> GenerateAveragedMethodDictionary(ConcurrentDictionary<string, ConcurrentDictionary<string, ConcreteMethodTelemetry>> telemetryData, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcreteMethodException>> exceptionData)
        {
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

        public IDictionary<string, AveragedMethod> GetAveragedMemberTelemetry()
        {
            if (_task == null)
            {
                StartBackgroundTask(CancellationToken.None);
            }
            return _currentAveragedMemberTelemetry;
        }
    }
}
