using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.DataCollection
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
