using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.Insights
{
    public class InsightsRestApiSessionData
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
