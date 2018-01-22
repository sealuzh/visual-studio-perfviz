using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using InSituVisualization.TelemetryCollector.DataPulling;
using InSituVisualization.TelemetryCollector.Filter;
using InSituVisualization.TelemetryCollector.Filter.Property;
using InSituVisualization.TelemetryCollector.Persistance;

namespace InSituVisualization.TelemetryCollector
{
    internal class AzureTelemetryStore : ITelemetryDataProvider
    {
        //private readonly AzureTelemetry _azureTelemetry;

        private static IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> _allMemberTelemetries;
        private static IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> _currentMemberTelemetries;

        private ConcurrentDictionary<string, AveragedMethodTelemetry> _currentAveragedMemberTelemetry;
        private Timer timer;
        private const int Timerinterval = 5000;

        private readonly FilterController _filterController;
        private readonly IList<IDataPullingService> _dataPullingServices;

        public AzureTelemetryStore()
        {
            _filterController = new FilterController();

            //As second attribute return the position inside the _filterController.GetFilterProperties()[0].GetFilterKinds() list, which is used for displaying possible filterparameters.
            _filterController.AddFilterGlobal(_filterController.GetFilterProperties()[1], FilterKind.IsGreaterEqualThen, new DateTime(2017, 11, 21));
            //_filterController.AddFilterGlobal(_filterController.GetFilterProperties()[2], IntFilterProperty.IsGreaterEqualThen, 1000);
            //_filterController.AddFilterLocal(_filterController.GetFilterProperties()[2], IntFilterProperty.IsGreaterEqualThen, 100, "ASP.testbuttonpage_aspx.Counter2");

            _dataPullingServices = new List<IDataPullingService> { new InsightsExternalReferencesRestApiDataPullingService() };
            //TODO JO: After FetchingSystemCacheData is called, the store is not updated.
            _allMemberTelemetries = new Dictionary<string, IDictionary<string, ConcreteMethodTelemetry>>();
            _allMemberTelemetries = PersistanceService.FetchSystemCacheData();

            if (_allMemberTelemetries.Any())
            {
                UpdateStore(false);
            }

            //Setup Timer Task that automatically updates the store via REST
            timer = new Timer
            {
                Interval = Timerinterval,
                AutoReset = false   
            };
            timer.Elapsed += RunPipeline;
            timer.Enabled = true;
        }

        private async void RunPipeline(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                var updateOccured = false;
                foreach (IDataPullingService service in _dataPullingServices)
                {
                    var newRestData = service.GetNewTelemetriesTaskAsync();
                    await newRestData;
                    //await PersistanceService.AwaitConcreteMemberTelemetriesLock();
                    PersistanceService.IsConcreteMemberTelemetriesLock = true;
                    foreach (ConcreteMethodTelemetry restReturnMember in newRestData.Result)
                    {
                        if (_allMemberTelemetries.ContainsKey(restReturnMember.DocumentationCommentId))
                        {
                            if (!_allMemberTelemetries[restReturnMember.DocumentationCommentId].ContainsKey(restReturnMember.Id))
                            {
                                //element is missing --> new element. Add it to the dict
                                _allMemberTelemetries[restReturnMember.DocumentationCommentId]
                                    .Add(restReturnMember.Id, restReturnMember);
                                updateOccured = true;
                            } //else: already exists, no need to add it

                        }
                        else
                        {
                            //case methodname does not exist: add a new dict for the new method, put the element inside.
                            var newDict =
                                new Dictionary<string, ConcreteMethodTelemetry>
                                {
                                    {restReturnMember.Id, restReturnMember}
                                };
                            _allMemberTelemetries.Add(restReturnMember.DocumentationCommentId, newDict);
                            updateOccured = true;
                        }

                    }
                    PersistanceService.IsConcreteMemberTelemetriesLock = false;
                }
                if (updateOccured)
                {
                    UpdateStore(true);
                }
                timer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void UpdateStore(bool persist)
        {
            if (persist) PersistanceService.WriteSystemCacheData(_allMemberTelemetries);
            _currentMemberTelemetries = _filterController.ApplyFilters(_allMemberTelemetries);
            _currentAveragedMemberTelemetry = TakeAverageOfDict(_currentMemberTelemetries);
        }

        private static ConcurrentDictionary<string, AveragedMethodTelemetry> TakeAverageOfDict(IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> inputDict)
        {
            //await PersistanceService.AwaitAverageMemberTelemetryLock();
            //PersistanceService.IsAverageTelemetryLock = true;
            var averagedDictionary = new ConcurrentDictionary<string, AveragedMethodTelemetry>();
            foreach (var method in inputDict.Values)
            {
                var timeList = new List<double>();

                foreach (var telemetry in method.Values)
                {
                    timeList.Add(telemetry.Duration);
                }
                if (method.Values.Count > 0)
                {
                    if (!averagedDictionary.TryAdd(method.Values.ElementAt(0).DocumentationCommentId,
                        new AveragedMethodTelemetry(method.Values.ElementAt(0).DocumentationCommentId,
                            TimeSpan.FromMilliseconds(timeList.Average()), timeList.Count, method)))
                    {
                        Console.WriteLine("Could not add element to dictionary with key " + method.Values.ElementAt(0).DocumentationCommentId);
                    }
                }
            }
            //PersistanceService.IsAverageTelemetryLock = false;
            return averagedDictionary;
        }

        public Dictionary<string, AveragedMethodTelemetry> GetAveragedMemberTelemetry()
        {
            return !PersistanceService.IsAverageTelemetryLock ? new Dictionary<string, AveragedMethodTelemetry>(_currentAveragedMemberTelemetry) : null;
        }

        public FilterController GetFilterController()
        {
            return _filterController;
        }

    }
}
