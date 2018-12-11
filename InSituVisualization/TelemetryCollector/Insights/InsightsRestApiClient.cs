using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.Insights
{
    /// <summary>
    /// See Azure Application Insights REST API
    /// https://dev.applicationinsights.io/apiexplorer/events
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global, Justification: IoC
    internal class InsightsRestApiClient
    {
        private const string QueryType = "events";
        private const string QueryPath = "dependencies";
        private const string ParameterString = "timespan=P60D&$orderby=timestamp%20desc";

        private readonly Uri _baseUri = new Uri("https://api.applicationinsights.io/v1/apps/");

        private readonly string _appId;
        private readonly string _apiKey;
        private readonly int _top;

        public InsightsRestApiClient(Settings settings)
        {
            _appId = settings.Options.AppId;
            _apiKey = settings.Options.ApiKey;
            _top = settings.Options.MaxPullingAmount;
        }

        public async Task<InsightsRestApiResponse> GetTelemetryAsync()
        {
            var parameters = ParameterString + $"&$top={_top}";
            return await GetTelemetryInternalAsync(parameters).ConfigureAwait(false);
        }

        public async Task<InsightsRestApiResponse> GetTelemetryAsync(string documentationCommentId)
        {
            var filter = $"$filter=dependency/name eq '{WebUtility.HtmlEncode(documentationCommentId)}'";
            var parameters = ParameterString + $"&$top={_top}&{filter}";
            return await GetTelemetryInternalAsync(parameters).ConfigureAwait(false);
        }

        private async Task<InsightsRestApiResponse> GetTelemetryInternalAsync(string parameters)
        {
            var telemetryJson = await GetStringAsync(parameters).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<InsightsRestApiResponse>(telemetryJson);
        }

        private async Task<string> GetStringAsync(string parameterString)
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_appId))
            {
                throw new InvalidOperationException("No AppID or ApiKey specified");
            }

            var client = new HttpClient
            {
                BaseAddress = _baseUri,
                Timeout = TimeSpan.FromMilliseconds(10000)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            var req = $"{_appId}/{QueryType}/{QueryPath}?{parameterString}";
            var response = await client.GetAsync(req).ConfigureAwait(false);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync().ConfigureAwait(false) : throw new InvalidOperationException(response.ReasonPhrase);
        }
    }
}
