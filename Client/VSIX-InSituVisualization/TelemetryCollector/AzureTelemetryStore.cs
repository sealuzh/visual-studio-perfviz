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
        private String _apiKey;
        private String _appId;
        private String _basePath;
        private readonly Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>> _concreteMemberTelemetries;
        private Dictionary<String, TimeSpan> _averageMemberTelemetry;
        private readonly int TIMERINTERVAL = 5000;
        private Boolean _isAverageTelemetryLock;
        private Boolean _isConcreteMemberTelemetriesLock;
        
        public AzureTelemetryStore(String appId, String apiKey)
        {
            _appId = appId;
            _apiKey = apiKey;
            _basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _isAverageTelemetryLock = false;
            _isConcreteMemberTelemetriesLock = false;
           
            _azureTelemetry = new AzureTelemetry(appId, apiKey);

            _concreteMemberTelemetries = FetchSystemCacheData();
            //_averageMemberTelemetry = new Dictionary<String, TimeSpan>();
            _averageMemberTelemetry = TakeAverageOfDict(_concreteMemberTelemetries);

            //TODO: Call stored data

            //Setup Timer Task that automatically updates the store via REST
            var timer = new Timer
            {
                Interval = (TIMERINTERVAL)
            };
            timer.Elapsed += FetchNewRestData;
            timer.Enabled = true;
            
        }


        public Dictionary<String, TimeSpan> GetAverageMemberTelemetry()
        {
            if (!_isAverageTelemetryLock)
            {
                return _averageMemberTelemetry;
            }
            else
            {
                return null;
            }
        }

        private Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>> FetchSystemCacheData()
        {
            if (System.IO.File.Exists(_basePath + "\\VSIXStore.json"))
            {
                String input = System.IO.File.ReadAllText(_basePath + "\\VSIXStore.json");
                Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>> importedConcreteMemberTelemetires =
                    JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>>>(input);
                return importedConcreteMemberTelemetires;
            }
            else
            {
                return new Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>>(); //first dict: Key Membername, second dict: Key RestSendID
            }
        }

        private void WriteSystemCacheData()
        {
            //TODO: Find better path because this one is deleted upon startup.
            String json = JsonConvert.SerializeObject(_concreteMemberTelemetries);
            System.IO.File.WriteAllText(_basePath + "\\VSIXStore.json", json);
        }

        private async void FetchNewRestData(object sender, ElapsedEventArgs e)
        {
            try
            {
                Boolean updateOccured = false;
                var newRestData = await _azureTelemetry.GetNewTelemetriesTaskAsync();
                AwaitConcreteMemberTelemetriesLock();
                _isConcreteMemberTelemetriesLock = true;
                foreach (ConcreteMemberTelemetry restReturnMember in newRestData)
                {
                    if (_concreteMemberTelemetries.ContainsKey(restReturnMember.MemberName))
                    {
                        if (!_concreteMemberTelemetries[restReturnMember.MemberName].ContainsKey(restReturnMember.Id))
                        {
                            //element is missing --> new element. Add it to the dict
                            _concreteMemberTelemetries[restReturnMember.MemberName].Add(restReturnMember.Id, restReturnMember);
                            updateOccured = true;
                        } //else: already exists, no need to add it

                    }
                    else
                    {
                        //case methodname does not exist: add a new dict for the new method, put the element inside.
                        Dictionary<String, ConcreteMemberTelemetry> newDict = new Dictionary<String, ConcreteMemberTelemetry>();
                        newDict.Add(restReturnMember.Id, restReturnMember);
                        _concreteMemberTelemetries.Add(restReturnMember.MemberName, newDict);
                        updateOccured = true;
                    }

                }
                _isConcreteMemberTelemetriesLock = false;
                if (updateOccured)
                {
                    _averageMemberTelemetry = TakeAverageOfDict(_concreteMemberTelemetries);
                    WriteSystemCacheData();
                }
                
            }
            catch (Exception em)
            {
                System.Console.WriteLine(em);
            }
            
        }

        private Dictionary<String, TimeSpan> TakeAverageOfDict(Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>> inputDict)
        {
            AwaitAverageMemberTelemetryLock();
            _isAverageTelemetryLock = true;
            Dictionary<String, TimeSpan> averagedDictionary = new Dictionary<String, TimeSpan>();
            foreach (Dictionary<String, ConcreteMemberTelemetry> method in inputDict.Values)
            {
                List<double> timeList = new List<double>();
                string memberName = "";
                foreach (ConcreteMemberTelemetry telemetry in method.Values)
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
