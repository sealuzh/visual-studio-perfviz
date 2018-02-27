using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    /// <summary>
    /// See Azure Application Insights REST API
    /// https://dev.applicationinsights.io/apiexplorer/events
    /// </summary>
    internal class InsightsRestApiDataCollector : IDataCollector
    {
        private const string Url = "https://api.applicationinsights.io/v1/apps/{0}/{1}/{2}?{3}";

        private const string QueryType = "events";
        private const string QueryPath = "dependencies";
        private const string ParameterString = "timespan=P30D&$orderby=timestamp%20desc";

        private readonly string _appId;
        private readonly string _apiKey;
        private readonly int _maxPullingAmount;

        public InsightsRestApiDataCollector(string appId, string apiKey, int maxPullingAmount)
        {
            _appId = appId;
            _apiKey = apiKey;
            _maxPullingAmount = maxPullingAmount;
        }

        public async Task<IList<CollectedDataEntity>> GetNewTelemetriesTaskAsync()
        {
            //check whether necessary variables are given - if not abort
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_appId))
            {
                return null;
            }

            var telemetryJson = await GetTelemetryAsync(_appId, _apiKey, QueryType, QueryPath, ParameterString + "&$top=" + _maxPullingAmount);
            dynamic telemetryData = JsonConvert.DeserializeObject(telemetryJson);

            var performanceInfoList = new List<CollectedDataEntity>();
            foreach (var obj in telemetryData.value.Children())
            {

                var performanceInfo = new CollectedDataEntity(obj);
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
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : throw new InvalidOperationException(response.ReasonPhrase);
        }
    }
}
