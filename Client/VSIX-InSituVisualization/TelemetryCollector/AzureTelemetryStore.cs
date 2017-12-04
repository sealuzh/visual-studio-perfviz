using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;

namespace VSIX_InSituVisualization.TelemetryCollector
{

    class AzureTelemetryStore
    {
        private readonly AzureTelemetry _azureTelemetry;
        private string _apiKey;
        private string _appId;
        private readonly string _basePath;
        private readonly IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> _allMemberTelemetries;
        private IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> _currentMemberTelemetries;
        private readonly TelemetryFilter _filter;
        private Dictionary<string, TimeSpan> _currentAveragedMemberTelemetry;
        private const int Timerinterval = 5000;
        private bool _isAverageTelemetryLock;
        private bool _isConcreteMemberTelemetriesLock;
        
        public AzureTelemetryStore(string appId, string apiKey)
        {
            _appId = appId;
            _apiKey = apiKey;
            _basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _isAverageTelemetryLock = false;
            _isConcreteMemberTelemetriesLock = false;
            _filter = new TelemetryFilter();

            AddFilter(GetFilterProperties()["City"], "IsEqual", "Zurich");
            AddFilter(GetFilterProperties()["Timestamp"], "IsGreaterEqualThen", new DateTime(2017, 11, 21));

            _azureTelemetry = new AzureTelemetry(appId, apiKey);

            _allMemberTelemetries = FetchSystemCacheData();
            _currentMemberTelemetries = _filter.ApplyFilters(_allMemberTelemetries);
            _currentAveragedMemberTelemetry = TakeAverageOfDict(_currentMemberTelemetries);

            //Setup Timer Task that automatically updates the store via REST
            var timer = new Timer
            {
                Interval = Timerinterval
            };
            timer.Elapsed += FetchNewRestData;
            timer.Enabled = true;
            
        }

        public Dictionary<string, TimeSpan> GetCurrentAveragedMemberTelemetry()
        {
            if (!_isAverageTelemetryLock)
            {
                return _currentAveragedMemberTelemetry;
            }
            return null;
        }

        public Dictionary<string, PropertyInfo> GetFilterProperties()
        {
            return _filter.GetFilterProperties();
        }

        public void ResetFilter()
        {
            _filter.ResetFilter();
        }

        public void AddFilter(PropertyInfo propertyInfo, string filterType, object parameter)
        {
            _filter.AddFilter(propertyInfo, filterType, parameter);
        }

        private IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> FetchSystemCacheData()
        {
            if (File.Exists(_basePath + "\\VSIXStore.json"))
            {
                var input = File.ReadAllText(_basePath + "\\VSIXStore.json");
                var importedConcreteMemberTelemetires = JsonConvert.DeserializeObject<Dictionary<string, IDictionary<string, ConcreteMemberTelemetry>>>(input);
                return importedConcreteMemberTelemetires;
            }
            return new Dictionary<string, IDictionary<string, ConcreteMemberTelemetry>>(); //first dict: Key Membername, second dict: Key RestSendID
        }

        private void WriteSystemCacheData(IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> toStoreTelemetryData)
        {
            //TODO: Find better path because this one is deleted upon startup.
            var json = JsonConvert.SerializeObject(toStoreTelemetryData);
            File.WriteAllText(_basePath + "\\VSIXStore.json", json);
        }

        private async void FetchNewRestData(object sender, ElapsedEventArgs e)
        {
            try
            {
                var updateOccured = false;
                var newRestData = await _azureTelemetry.GetNewTelemetriesTaskAsync();
                AwaitConcreteMemberTelemetriesLock();
                _isConcreteMemberTelemetriesLock = true;
                foreach (ConcreteMemberTelemetry restReturnMember in newRestData)
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
                        var newDict = new Dictionary<string, ConcreteMemberTelemetry>();
                        newDict.Add(restReturnMember.Id, restReturnMember);
                        _allMemberTelemetries.Add(restReturnMember.MemberName, newDict);
                        updateOccured = true;
                    }

                }
                _isConcreteMemberTelemetriesLock = false;
                if (updateOccured)
                {
                    WriteSystemCacheData(_allMemberTelemetries);
                    _currentMemberTelemetries = _filter.ApplyFilters(_allMemberTelemetries);
                    _currentAveragedMemberTelemetry = TakeAverageOfDict(_currentMemberTelemetries);
                    
                }
                
            }
            catch (Exception em)
            {
                Console.WriteLine(em);
            }
            
        }

        private Dictionary<string, TimeSpan> TakeAverageOfDict(IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> inputDict)
        {
            AwaitAverageMemberTelemetryLock();
            _isAverageTelemetryLock = true;
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
            _isAverageTelemetryLock = false;
            return averagedDictionary;
        }

        private void AwaitConcreteMemberTelemetriesLock()
        {
            while (_isConcreteMemberTelemetriesLock)
            {
                Task.Delay(50);
            }
        }

        private void AwaitAverageMemberTelemetryLock()
        {
            while (_isAverageTelemetryLock)
            {
                Task.Delay(50);
            }
        }

    }
}
