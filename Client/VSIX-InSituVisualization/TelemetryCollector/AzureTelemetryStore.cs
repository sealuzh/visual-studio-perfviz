﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using VSIX_InSituVisualization.TelemetryCollector.DataService;
using VSIX_InSituVisualization.TelemetryCollector.Filter;
using VSIX_InSituVisualization.TelemetryCollector.Persistance;

namespace VSIX_InSituVisualization.TelemetryCollector
{

    class AzureTelemetryStore
    {
        //private readonly AzureTelemetry _azureTelemetry;

        private static IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> _allMemberTelemetries;
        private static IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> _currentMemberTelemetries;
        
        private Dictionary<string, TimeSpan> _currentAveragedMemberTelemetry;
        private const int Timerinterval = 5000;

        private readonly FilterController _filterController;
        private readonly IList<IDataService> _dataServices;
        
        public AzureTelemetryStore()
        {
           _filterController = new FilterController();
            _filterController.AddFilter(GetFilterProperties()["Timestamp"], "IsGreaterEqualThen", new DateTime(2017, 11, 21));

            _dataServices = new List<IDataService>();
            _dataServices.Add(new InsightsExternalReferencesRestApiDataService());
            
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
            var updateOccured = false;
            foreach (IDataService service in _dataServices)
            {
                var newRestData = service.GetNewTelemetriesTaskAsync();
                await newRestData;
                PersistanceService.AwaitConcreteMemberTelemetriesLock();
                PersistanceService.IsConcreteMemberTelemetriesLock = true;
                foreach (ConcreteMemberTelemetry restReturnMember in newRestData.Result)
                {
                    if (_allMemberTelemetries.ContainsKey(restReturnMember.MemberName))
                    {
                        if (!_allMemberTelemetries[restReturnMember.MemberName].ContainsKey(restReturnMember.Id))
                        {
                            //element is missing --> new element. Add it to the dict
                            _allMemberTelemetries[restReturnMember.MemberName].Add(restReturnMember.Id, restReturnMember);
                            updateOccured = true;
                        } //else: already exists, no need to add it

                    }
                    else
                    {
                        //case methodname does not exist: add a new dict for the new method, put the element inside.
                        var newDict = new Dictionary<string, ConcreteMemberTelemetry> { { restReturnMember.Id, restReturnMember } };
                        _allMemberTelemetries.Add(restReturnMember.MemberName, newDict);
                        updateOccured = true;
                    }

                }
                PersistanceService.IsConcreteMemberTelemetriesLock = false;  
            }
            if (updateOccured) UpdateStore(true);
        }

        public Dictionary<string, TimeSpan> GetCurrentAveragedMemberTelemetry()
        {
            return !PersistanceService.IsAverageTelemetryLock ? new Dictionary<string, TimeSpan>(_currentAveragedMemberTelemetry) : null;
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

        //public void AddFilter(PropertyInfo propertyInfo, string filterType, object parameter)
        //{
        //    _filterController.AddFilter(propertyInfo, filterType, parameter);
        //}

        private void UpdateStore(bool persist)
        {
            if (persist) PersistanceService.WriteSystemCacheData(_allMemberTelemetries);
            _currentMemberTelemetries = _filterController.ApplyFilters(_allMemberTelemetries);
            _currentAveragedMemberTelemetry = TakeAverageOfDict(_currentMemberTelemetries);
        }

        

        //private async void FetchNewRestData(object sender, ElapsedEventArgs e)
        //{
        //    var updateOccured = false;
        //    var newRestData = await _azureTelemetry.GetNewTelemetriesTaskAsync();
        //    PersistanceService.AwaitConcreteMemberTelemetriesLock();
        //    PersistanceService.IsConcreteMemberTelemetriesLock = true;
        //    foreach (ConcreteMemberTelemetry restReturnMember in newRestData)
        //    {
        //        if (_allMemberTelemetries.ContainsKey(restReturnMember.MemberName))
        //        {
        //            if (!_allMemberTelemetries[restReturnMember.MemberName].ContainsKey(restReturnMember.Id))
        //            {
        //                //element is missing --> new element. Add it to the dict
        //                _allMemberTelemetries[restReturnMember.MemberName].Add(restReturnMember.Id, restReturnMember);
        //                updateOccured = true;
        //            } //else: already exists, no need to add it

        //        }
        //        else
        //        {
        //            //case methodname does not exist: add a new dict for the new method, put the element inside.
        //            var newDict = new Dictionary<string, ConcreteMemberTelemetry> { { restReturnMember.Id, restReturnMember } };
        //            _allMemberTelemetries.Add(restReturnMember.MemberName, newDict);
        //            updateOccured = true;
        //        }

        //    }
        //    PersistanceService.IsConcreteMemberTelemetriesLock = false;
        //    if (updateOccured) UpdateStore(true);
        //}

        private Dictionary<string, TimeSpan> TakeAverageOfDict(IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> inputDict)
        {
            PersistanceService.AwaitAverageMemberTelemetryLock();
            PersistanceService.IsAverageTelemetryLock = true;
            var averagedDictionary = new Dictionary<string, TimeSpan>();
            foreach (var method in inputDict.Values)
            {
                var timeList = new List<double>();
                var memberName = "";
                foreach (var telemetry in method.Values)
                {
                    timeList.Add(telemetry.Duration.TotalMilliseconds);
                    memberName = telemetry.MemberName;
                }
                averagedDictionary.Add(memberName, TimeSpan.FromMilliseconds(timeList.Average()));
            }
            PersistanceService.IsAverageTelemetryLock = false;
            return averagedDictionary;
        }



    }
}
