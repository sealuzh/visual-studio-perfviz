using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using VSIX_InSituVisualization.TelemetryCollector.DataPulling;
using VSIX_InSituVisualization.TelemetryCollector.Filter;
using VSIX_InSituVisualization.TelemetryCollector.Persistance;

namespace VSIX_InSituVisualization.TelemetryCollector
{

    class AzureTelemetryStore
    {
        //private readonly AzureTelemetry _azureTelemetry;

        private static IDictionary<string, IDictionary<string, ConcreteTelemetryMember>> _allMemberTelemetries;
        private static IDictionary<string, IDictionary<string, ConcreteTelemetryMember>> _currentMemberTelemetries;
        
        private Dictionary<string, AveragedTelemetry> _currentAveragedMemberTelemetry;
        private const int Timerinterval = 5000;

        private readonly FilterController _filterController;
        private readonly IList<IDataPullingService> _dataPullingServices;
        
        public AzureTelemetryStore()
        {
            _filterController = new FilterController();
            _filterController.AddFilter(GetFilterProperties()["Timestamp"], "IsGreaterEqualThen", new DateTime(2017, 11, 21));

            _dataPullingServices = new List<IDataPullingService>();
            _dataPullingServices.Add(new InsightsExternalReferencesRestApiDataPullingService());
            
            _allMemberTelemetries = new Dictionary<string, IDictionary<string, ConcreteTelemetryMember>>();
            _allMemberTelemetries = PersistanceService.FetchSystemCacheData();
            UpdateStore(false);

            //Setup Timer Task that automatically updates the store via REST
            var timer = new Timer
            {
                Interval = Timerinterval
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
                    PersistanceService.AwaitConcreteMemberTelemetriesLock();
                    PersistanceService.IsConcreteMemberTelemetriesLock = true;
                    foreach (ConcreteTelemetryMember restReturnMember in newRestData.Result)
                    {
                        if (_allMemberTelemetries.ContainsKey(restReturnMember.MemberName))
                        {
                            if (!_allMemberTelemetries[restReturnMember.MemberName].ContainsKey(restReturnMember.Id))
                            {
                                //element is missing --> new element. Add it to the dict
                                _allMemberTelemetries[restReturnMember.MemberName]
                                    .Add(restReturnMember.Id, restReturnMember);
                                updateOccured = true;
                            } //else: already exists, no need to add it

                        }
                        else
                        {
                            //case methodname does not exist: add a new dict for the new method, put the element inside.
                            var newDict =
                                new Dictionary<string, ConcreteTelemetryMember>
                                {
                                    {restReturnMember.Id, restReturnMember}
                                };
                            _allMemberTelemetries.Add(restReturnMember.MemberName, newDict);
                            updateOccured = true;
                        }

                    }
                    PersistanceService.IsConcreteMemberTelemetriesLock = false;
                }
                if (updateOccured) UpdateStore(true);
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

        private Dictionary<string, AveragedTelemetry> TakeAverageOfDict(
            IDictionary<string, IDictionary<string, ConcreteTelemetryMember>> inputDict)
        {
            PersistanceService.AwaitAverageMemberTelemetryLock();
            PersistanceService.IsAverageTelemetryLock = true;
            var averagedDictionary = new Dictionary<string, AveragedTelemetry>();
            foreach (var method in inputDict.Values)
            {
                var timeList = new List<double>();
                
                foreach (var telemetry in method.Values)
                {
                    timeList.Add(telemetry.Duration.TotalMilliseconds);
                }
                averagedDictionary.Add(method.Values.ElementAt(0).MemberName, new AveragedTelemetry(method.Values.ElementAt(0).MemberName, method.Values.ElementAt(0).NameSpace, TimeSpan.FromMilliseconds(timeList.Average()), timeList.Count()));
            }
            PersistanceService.IsAverageTelemetryLock = false;
            return averagedDictionary;
        }

        public Dictionary<string, AveragedTelemetry> GetCurrentAveragedMemberTelemetry()
        {
            return !PersistanceService.IsAverageTelemetryLock ? new Dictionary<string, AveragedTelemetry>(_currentAveragedMemberTelemetry) : null;
        }

        public void AddFilter(PropertyInfo propertyInfo, string filterType, object parameter)
        {
            _filterController.AddFilter(propertyInfo, filterType, parameter);
        }

        public Dictionary<string, PropertyInfo> GetFilterProperties()
        {
            return _filterController.GetFilterProperties();
        }

        public void ResetFilter()
        {
            _filterController.ResetFilter();
            UpdateStore(false);
        }
    }
}
