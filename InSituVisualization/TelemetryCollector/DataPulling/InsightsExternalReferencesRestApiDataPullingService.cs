using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;
using InSituVisualization.TelemetryCollector.Persistance;
using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.DataPulling
{
    class InsightsExternalReferencesRestApiDataPullingService : IDataPullingService
    {
        private const string Url = "https://api.applicationinsights.io/v1/apps/{0}/{1}/{2}?{3}";

        private const string QueryType = "events";
        private const string QueryPath = "dependencies";
        private readonly string _parameterString = "timespan=P30D&$orderby=timestamp%20desc";

        private readonly string _appId;
        private readonly string _apiKey;
        

        public InsightsExternalReferencesRestApiDataPullingService()
        {
            _appId = (string)WritableSettingsStoreController.GetWritableSettingsStoreValue("Performance Visualization", "AppId", typeof(string));
            _apiKey = (string)WritableSettingsStoreController.GetWritableSettingsStoreValue("Performance Visualization", "ApiKey", typeof(string));
            var maxPullingAmount = (int) WritableSettingsStoreController.GetWritableSettingsStoreValue("Performance Visualization", "MaxPullingAmount", typeof(int));
            
            _parameterString = _parameterString + "&$top=" + maxPullingAmount;
        }

        public async Task<IList<PulledDataEntity>> GetNewTelemetriesTaskAsync()
        {
            //check whether necessary variables are given - if not abort
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_appId))
            {
                return null;
            }

            var telemetryJson = await GetTelemetryAsync(_appId, _apiKey, QueryType, QueryPath, _parameterString);
            dynamic telemetryData = JsonConvert.DeserializeObject(telemetryJson);

            var performanceInfoList = new List<PulledDataEntity>();
            foreach (var obj in telemetryData.value.Children())
            {

                var performanceInfo = new PulledDataEntity(obj);
                performanceInfoList.Add(performanceInfo);
                
            }
            return performanceInfoList;
        }

        private static async Task<string> GetTelemetryAsync(string appid, string apikey, string queryType, string queryPath, string parameterString)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", apikey);
            var req = string.Format(Url, appid, queryType, queryPath, parameterString);
            var response = await client.GetAsync(req);
            return response.IsSuccessStatusCode ? response.Content.ReadAsStringAsync().Result : throw new InvalidOperationException(response.ReasonPhrase);
        }
    }
}
