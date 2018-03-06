using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.Insights
{
    public class InsightsRestApiUserData
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// accountId
        /// </summary>
        [JsonProperty(PropertyName = "accountId")]
        public string AccountId { get; set; }

        /// <summary>
        /// authenticatedId
        /// </summary>
        [JsonProperty(PropertyName = "authenticatedId")]
        public string AuthenticatedId { get; set; }
    }
}
