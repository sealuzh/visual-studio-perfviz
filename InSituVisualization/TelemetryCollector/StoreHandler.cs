using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using InSituVisualization.TelemetryCollector.DataPulling;
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
                    
                    foreach (ConcreteMethodTelemetry restReturnMember in newRestData.Result)
                    {
                        switch (restReturnMember.Type)
                        {
                            case "Telemetry":
                                if (StoreProvider.GetTelemetryStore().GetAllMethodTelemetries()
                                    .ContainsKey(restReturnMember.DocumentationCommentId))
                                {
                                    {
                                        if (!StoreProvider.GetTelemetryStore().GetAllMethodTelemetries()[restReturnMember.DocumentationCommentId].ContainsKey(restReturnMember.Id))
                                        {
                                            //element is missing --> new element. Add it to the dict
                                            if (!StoreProvider.GetTelemetryStore().GetAllMethodTelemetries()[restReturnMember.DocumentationCommentId]
                                                .TryAdd(restReturnMember.Id, restReturnMember))
                                            {
                                                Console.WriteLine("Could not add element " + restReturnMember.DocumentationCommentId);
                                            }
                                            updateOccured = true;
                                        } //else: already exists, no need to add it
                                    }
                                }
                                else
                                {
                                    //case methodname does not exist: add a new dict for the new method, put the element inside.
                                    var newDict = new ConcurrentDictionary<string, ConcreteMethodTelemetry>();
                                    if (!newDict.TryAdd(restReturnMember.Id, restReturnMember))
                                    {
                                        Console.WriteLine("Could not add dict " + restReturnMember.Id);
                                    };
                                    if (!StoreProvider.GetTelemetryStore().GetAllMethodTelemetries()
                                        .TryAdd(restReturnMember.DocumentationCommentId, newDict))
                                    {
                                        Console.WriteLine("Could not add dict " + restReturnMember.DocumentationCommentId);  
                                    }
                                    updateOccured = true;
                                }
                                break;
                            case "Exception":
                                //TODO: Implement Exception Store
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
            var averagedDictionary = new ConcurrentDictionary<string, AveragedMethod>();
            foreach (var key in telemetryData.Keys)
            {
                if (exceptionData.ContainsKey(key))
                {
                    if (!averagedDictionary.TryAdd(telemetryData[key].Values.ElementAt(0).DocumentationCommentId,
                        new AveragedMethod(telemetryData[key].Values.ElementAt(0).DocumentationCommentId, telemetryData[key], exceptionData[key])))
                    {
                        Console.WriteLine("Could not add element to dictionary with key " +
                                          telemetryData[key].Values.ElementAt(0).DocumentationCommentId);
                    }
                }
                else
                {
                    if (!averagedDictionary.TryAdd(telemetryData[key].Values.ElementAt(0).DocumentationCommentId,
                        new AveragedMethod(telemetryData[key].Values.ElementAt(0).DocumentationCommentId, telemetryData[key])))
                    {
                        Console.WriteLine("Could not add element to dictionary with key " +
                                          telemetryData[key].Values.ElementAt(0).DocumentationCommentId);
                    }
                }
                

            }
            return averagedDictionary;
        }

        public ConcurrentDictionary<string, AveragedMethod> GetAveragedMemberTelemetry() => CurrentAveragedMemberTelemetry;

    }
}
