using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using VSIX_InSituVisualization.TelemetryCollector.DataPulling;
using VSIX_InSituVisualization.TelemetryCollector.Filter;
using VSIX_InSituVisualization.TelemetryCollector.Filter.Property;
using VSIX_InSituVisualization.TelemetryCollector.Persistance;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    internal class AzureTelemetryStore : ITelemetryDataProvider
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

            //As second attribute return the position inside the _filterController.GetFilterProperties()[0].GetFilterParameterList() list, which is used for displaying possible filterparameters.
            _filterController.AddFilterGlobal(_filterController.GetFilterProperties()[1], DateTimeFilterProperty.IsGreaterEqualThen, new DateTime(2017, 11, 21));
            _filterController.AddFilterGlobal(_filterController.GetFilterProperties()[6], IntFilterProperty.IsGreaterEqualThen, 100);
            _filterController.AddFilterLocal(_filterController.GetFilterProperties()[6], IntFilterProperty.IsGreaterEqualThen, 1000, "Counter2");

            _dataPullingServices = new List<IDataPullingService> { new InsightsExternalReferencesRestApiDataPullingService() };
            //TODO JO: After FetchingSystemCacheData is called, the store is not updated.
            _allMemberTelemetries = new Dictionary<string, IDictionary<string, ConcreteTelemetryMember>>();
            //_allMemberTelemetries = PersistanceService.FetchSystemCacheData();
            
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
                    await PersistanceService.AwaitConcreteMemberTelemetriesLock();
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
                if (updateOccured)
                {
                    await UpdateStore(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private async Task UpdateStore(bool persist)
        {
            if (persist) PersistanceService.WriteSystemCacheData(_allMemberTelemetries);
            _currentMemberTelemetries = _filterController.ApplyFilters(_allMemberTelemetries);
            _currentAveragedMemberTelemetry = await TakeAverageOfDict(_currentMemberTelemetries);
        }

        private static async Task<Dictionary<string, AveragedTelemetry>> TakeAverageOfDict(IDictionary<string, IDictionary<string, ConcreteTelemetryMember>> inputDict)
        {
            await PersistanceService.AwaitAverageMemberTelemetryLock();
            PersistanceService.IsAverageTelemetryLock = true;
            var averagedDictionary = new Dictionary<string, AveragedTelemetry>();
            foreach (var method in inputDict.Values)
            {
                var timeList = new List<double>();

                foreach (var telemetry in method.Values)
                {
                    timeList.Add(telemetry.Duration);
                }
                averagedDictionary.Add(method.Values.ElementAt(0).MemberName, new AveragedTelemetry(method.Values.ElementAt(0).MemberName, method.Values.ElementAt(0).NameSpace, TimeSpan.FromMilliseconds(timeList.Average()), timeList.Count()));
            }
            PersistanceService.IsAverageTelemetryLock = false;
            return averagedDictionary;
        }

        public Dictionary<string, AveragedTelemetry> GetAveragedMemberTelemetry()
        {
            return !PersistanceService.IsAverageTelemetryLock ? new Dictionary<string, AveragedTelemetry>(_currentAveragedMemberTelemetry) : null;
        }

        public FilterController GetFilterController()
        {
            return _filterController;
        }

    }
}
