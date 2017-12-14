using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VSIX_InSituVisualization.TelemetryCollector.DataPulling
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

        public async Task<IList<ConcreteTelemetryMember>> GetNewTelemetriesTaskAsync()
        {
            //check whether necessary variables are given - if not abort
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_appId))
            {
                return null;
            }

            var telemetryJson = await GetTelemetryAsync(_appId, _apiKey, QueryType, QueryPath, _parameterString);
            dynamic telemetryData = JsonConvert.DeserializeObject(telemetryJson);

            var performanceInfoList = new List<ConcreteTelemetryMember>();
            foreach (var obj in telemetryData.value.Children())
            {
                var performanceInfo = new ConcreteTelemetryMember(
                    (string)obj.id,
                    Convert.ToDateTime(obj.timestamp),
                    (string)obj.dependency.target,
                    TimeSpan.FromMilliseconds((double)obj.dependency.duration).Milliseconds,
                    (string)obj.client.city,
                    (string)obj.dependency.name,
                    (string)obj.dependency.type);
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
