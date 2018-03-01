using System;
using System.Collections.Generic;
using System.Net;
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
        private const string QueryType = "events";
        private const string QueryPath = "dependencies";
        private const string ParameterString = "timespan=P30D&$orderby=timestamp%20desc";

        private readonly Uri _baseUri = new Uri("https://api.applicationinsights.io/v1/apps/");

        private readonly string _appId;
        private readonly string _apiKey;
        private readonly int _maxPullingAmount;

        public InsightsRestApiDataCollector(string appId, string apiKey, int maxPullingAmount)
        {
            _appId = appId;
            _apiKey = apiKey;
            _maxPullingAmount = maxPullingAmount;
        }

        public async Task<IList<CollectedDataEntity>> GetTelemetryAsync()
        {
            var parameters = ParameterString + $"&$top={_maxPullingAmount}";
            return await GetTelemetryInternalAsync(parameters);
        }

        public async Task<IList<CollectedDataEntity>> GetTelemetryAsync(string documentationCommentId)
        {
            var filter = $"$filter=dependency/name eq '{WebUtility.HtmlEncode(documentationCommentId)}'";
            var parameters = ParameterString + $"&$top={_maxPullingAmount}&{filter}";
            return await GetTelemetryInternalAsync(parameters);
        }

        private async Task<IList<CollectedDataEntity>> GetTelemetryInternalAsync(string parameters)
        {
            var telemetryJson = await GetStringAsync(parameters);
            dynamic telemetryData = JsonConvert.DeserializeObject(telemetryJson);

            var performanceInfoList = new List<CollectedDataEntity>();
            foreach (var obj in telemetryData.value.Children())
            {
                performanceInfoList.Add(new CollectedDataEntity(obj));

            }
            return performanceInfoList;
        }

        private async Task<string> GetStringAsync(string parameterString)
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_appId))
            {
                throw new InvalidOperationException("No AppID or ApiKey specified");
            }

            var client = new HttpClient
            {
                BaseAddress = _baseUri
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            var req = $"{_appId}/{QueryType}/{QueryPath}?{parameterString}";
            var response = await client.GetAsync(req);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : throw new InvalidOperationException(response.ReasonPhrase);
        }
    }
}
