using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    internal class AzureTelemetry
    {
        private const string Url = "https://api.applicationinsights.io/v1/apps/{0}/{1}/{2}?{3}";

        private const string QueryType = "events";
        private const string QueryPath = "dependencies";
        private const string ParameterString = "timespan=P30D&$orderby=timestamp%20desc&$top=10";

        private readonly string _appId;
        private readonly string _apiKey;


        public AzureTelemetry(string appId, string apiKey)
        {
            _appId = appId;
            _apiKey = apiKey;
        }

        public async Task<IList<MemberTelemetry>> GetConcreteMemberTelemetriesAsync()
        {
            var telemetryJson = await GetTelemetryAsync(_appId, _apiKey, QueryType, QueryPath, ParameterString);
            dynamic telemetryData = JsonConvert.DeserializeObject(telemetryJson);

            var performanceInfoList = new List<MemberTelemetry>();
            foreach (dynamic obj in telemetryData.value.Children())
            {
                var performanceInfo = new ConcreteMemberTelemetry(
                    (string)obj.id,
                    Convert.ToDateTime(obj.timestamp),
                    (string)obj.dependency.name,
                    TimeSpan.FromMilliseconds((double)obj.dependency.duration));
                performanceInfoList.Add(performanceInfo);
            }
            return performanceInfoList;
        }

        public async Task<IList<AverageMemberTelemety>> GetAverageMemberTelemetiesAsync()
        {
            var concreteTelemetries = await GetConcreteMemberTelemetriesAsync();
            var groupedTelemetries = concreteTelemetries.GroupBy(tel => tel.MemberName);
            return groupedTelemetries.Select(grp => new AverageMemberTelemety(grp.Key, TimeSpan.FromMilliseconds(grp.Average(tel => tel.Duration.TotalMilliseconds)))).ToList();
        }

        private static async Task<string> GetTelemetryAsync(string appid, string apikey, string queryType, string queryPath, string parameterString)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", apikey);
            var req = string.Format(Url, appid, queryType, queryPath, parameterString);
            var response = await client.GetAsync(req);
            return response.IsSuccessStatusCode ? response.Content.ReadAsStringAsync().Result : response.ReasonPhrase;
        }

        
    }

    


}
