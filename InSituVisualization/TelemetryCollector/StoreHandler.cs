#define DEBUG

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using InSituVisualization.TelemetryCollector.DataPulling;
using InSituVisualization.TelemetryCollector.Filter.Property;
using InSituVisualization.TelemetryCollector.Model.AveragedMember;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;
using InSituVisualization.TelemetryCollector.Store;

namespace InSituVisualization.TelemetryCollector
{
    class StoreHandler : IStoreHandler
    {
        private const int Timerinterval = 5000;
        protected ConcurrentDictionary<string, AveragedMethod> CurrentAveragedMemberTelemetry;
        private readonly Timer _timer;

        public StoreHandler()
        {

#if DEBUG
            StoreProvider.GetTelemetryStore().GetFilterController().AddFilterGlobal(
                StoreProvider.GetTelemetryStore().GetFilterController().GetFilterProperties()[3],
                FilterKind.IsGreaterEqualThen, new DateTime(2018, 1, 15, 12, 45, 00));
#endif

            //first time build of averagedDictionary
            foreach (var store in StoreProvider.GetStores())
            {
                store.Update(false);
            }
            CurrentAveragedMemberTelemetry = GenerateAveragedMethodDictionary(StoreProvider.GetTelemetryStore().GetCurrentMethodTelemetries(), StoreProvider.GetExceptionStore().GetCurrentMethodTelemetries());

            _timer = new Timer
            {
                Interval = Timerinterval,
                AutoReset = false
            };
            _timer.Elapsed += RunPipeline;
            _timer.Enabled = true;
            
        }

        private async void RunPipeline(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                var updateOccured = false;
                foreach (IDataPullingService service in DataPullingServiceProdvider.GetDataPullingServices())
                {
                    var newRestData = service.GetNewTelemetriesTaskAsync();
                    await newRestData;
                    
                    foreach (PulledDataEntity restReturnMember in newRestData.Result)
                    {
                        switch (restReturnMember.Dependency.Name)
                        {
                            case "telemetry":
                                var telemetry = restReturnMember.GetConcreteMethodTelemetry();
                                if (StoreProvider.GetTelemetryStore().GetAllMethodTelemetries()
                                    .ContainsKey(telemetry.DocumentationCommentId))
                                {
                                    {
                                        if (!StoreProvider.GetTelemetryStore().GetAllMethodTelemetries()[telemetry.DocumentationCommentId].ContainsKey(telemetry.Id))
                                        {
                                            //element is missing --> new element. Add it to the dict
                                            if (!StoreProvider.GetTelemetryStore().GetAllMethodTelemetries()[telemetry.DocumentationCommentId]
                                                .TryAdd(telemetry.Id, telemetry))
                                            {
                                                Console.WriteLine("Could not add element " + telemetry.DocumentationCommentId);
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
                                        Console.WriteLine("Could not add dict " + telemetry.Id);
                                    };
                                    if (!StoreProvider.GetTelemetryStore().GetAllMethodTelemetries()
                                        .TryAdd(telemetry.DocumentationCommentId, newDict))
                                    {
                                        Console.WriteLine("Could not add dict " + telemetry.DocumentationCommentId);  
                                    }
                                    updateOccured = true;
                                }
                                break;
                            case "exception":
                                var exception = restReturnMember.GetConcreteMethodException();
                                if (StoreProvider.GetExceptionStore().GetAllMethodTelemetries()
                                    .ContainsKey(exception.DocumentationCommentId))
                                {
                                    {
                                        if (!StoreProvider.GetExceptionStore().GetAllMethodTelemetries()[exception.DocumentationCommentId].ContainsKey(exception.Id))
                                        {
                                            //element is missing --> new element. Add it to the dict
                                            if (!StoreProvider.GetExceptionStore().GetAllMethodTelemetries()[exception.DocumentationCommentId]
                                                .TryAdd(exception.Id, exception))
                                            {
                                                Console.WriteLine("Could not add element " + exception.DocumentationCommentId);
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
                                        Console.WriteLine("Could not add dict " + exception.Id);
                                    };
                                    if (!StoreProvider.GetExceptionStore().GetAllMethodTelemetries()
                                        .TryAdd(exception.DocumentationCommentId, newDict))
                                    {
                                        Console.WriteLine("Could not add dict " + exception.DocumentationCommentId);
                                    }
                                    updateOccured = true;
                                }
                                break;
                        }
                    }
                }
                if (updateOccured)
                {
                    foreach (var store in StoreProvider.GetStores())
                    {
                        store.Update(true);
                    }
                    CurrentAveragedMemberTelemetry = GenerateAveragedMethodDictionary(StoreProvider.GetTelemetryStore().GetCurrentMethodTelemetries(), StoreProvider.GetExceptionStore().GetCurrentMethodTelemetries());

                }
                _timer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _timer.Start();
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
                        Console.WriteLine("Could not add element to dictionary with key " + key);
                    }
                }
                else
                {
                    if (!averagedDictionary.TryAdd(key, new AveragedMethod(key, telemetryData[key])))
                    {
                        Console.WriteLine("Could not add element to dictionary with key " + key);
                    }
                }
            }
            return averagedDictionary;
        }

        public ConcurrentDictionary<string, AveragedMethod> GetAveragedMemberTelemetry() => CurrentAveragedMemberTelemetry;
    }
}
