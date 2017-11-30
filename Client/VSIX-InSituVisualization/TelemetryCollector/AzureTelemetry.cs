using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    //[ProvideBindingPath]
    internal class AzureTelemetry
    {
        private const string Url = "https://api.applicationinsights.io/v1/apps/{0}/{1}/{2}?{3}";

        private const string QueryType = "events";
        private const string QueryPath = "dependencies";
        private const string ParameterString = "timespan=P30D&$orderby=timestamp%20desc&$top=100";

        private readonly string _appId;
        private readonly string _apiKey;
       
        public AzureTelemetry(string appId, string apiKey)
        {
            _appId = appId;
            _apiKey = apiKey;
            
        }

        public async Task<IList<MemberTelemetry>> GetNewTelemetriesTaskAsync()
        {
            var telemetryJson = await GetTelemetryAsync(_appId, _apiKey, QueryType, QueryPath, ParameterString);
            dynamic telemetryData = JsonConvert.DeserializeObject(telemetryJson);

            var performanceInfoList = new List<MemberTelemetry>();
            foreach (dynamic obj in telemetryData.value.Children())
            {
                var performanceInfo = new ConcreteMemberTelemetry(
                    (string)obj.id,
                    Convert.ToDateTime(obj.timestamp),
                    (string)obj.dependency.target,
                    TimeSpan.FromMilliseconds((double)obj.dependency.duration),
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
