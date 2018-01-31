using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using InSituVisualization.TelemetryCollector.DataPulling;
using InSituVisualization.TelemetryCollector.Model.AveragedMember;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;
using InSituVisualization.TelemetryCollector.Store;

namespace InSituVisualization.TelemetryCollector
{
    class StoreHandler : IStoreHandler
    {
        //private ConcurrentDictionary<string, AveragedMethod> _currentAveragedMemberTelemetry;
        //private List<Timer> timers;
        private const int Timerinterval = 5000;

       //private readonly IList<IDataPullingService> _dataPullingServices;
       protected ConcurrentDictionary<string, AveragedMethod> CurrentAveragedMemberTelemetry;

        //private readonly IList<IStoreT> _stores;
        private readonly Timer _timer;

        public StoreHandler()
        {
            //FilterController.AddFilterGlobal(FilterController.GetFilterProperties()[1], FilterKind.IsGreaterEqualThen, new DateTime(2017, 11, 21));

            //_dataPullingServices = DataPullingServiceProdvider.GetDataPullingServices();

            _timer = new Timer
            {
                Interval = Timerinterval,
                AutoReset = false
            };
            _timer.Elapsed += RunPipeline;
            _timer.Enabled = true;

        }

        //public ConcurrentDictionary<string, AveragedMethod> GetAveragedMemberTelemetry()
        //{
        //    return _currentAveragedMemberTelemetry;
        //}

        private async void RunPipeline(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                var updateOccured = false;
                foreach (IDataPullingService service in DataPullingServiceProdvider.GetDataPullingServices())
                {
                    var newRestData = service.GetNewTelemetriesTaskAsync();
                    await newRestData;
                    //await PersistanceService.AwaitConcreteMemberTelemetriesLock();
                    //PersistanceService.IsConcreteMemberTelemetriesLock = true;
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
                    UpdateStore(true);
                }
                _timer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _timer.Start();
            }
        }

        private void UpdateStore(bool persist)
        {
            foreach (var store in StoreProvider.GetStores())
            {
                store.Update(persist);
            }
            CurrentAveragedMemberTelemetry = GenerateAveragedMethodDictionary(StoreProvider.GetTelemetryStore().GetCurrentMethodTelemetries(), StoreProvider.GetExceptionStore().GetCurrentMethodTelemetries());
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


        //private static ConcurrentDictionary<string, AveragedMethod> GenerateAveragedMethodDictionary(IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> telemetryData)
        //{
        //    var averagedDictionary = new ConcurrentDictionary<string, AveragedMethod>();
        //    foreach (var method in telemetryData.Values)
        //    {
        //        if (!averagedDictionary.TryAdd(method.Values.ElementAt(0).DocumentationCommentId,
        //            new AveragedMethod(method.Values.ElementAt(0).DocumentationCommentId, method)))
        //        {
        //            Console.WriteLine("Could not add element to dictionary with key " + method.Values.ElementAt(0).DocumentationCommentId);
        //        }

        //    }
        //    return averagedDictionary;
        ////}
        //public ConcurrentDictionary<string, AveragedMethod> GetAveragedMemberTelemetry()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
