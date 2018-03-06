using System;
using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public class InsightsRestApiResponseValue
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "session")]
        public InsightsRestApiSessionData Session { get; set; }

        [JsonProperty(PropertyName = "user")]
        public InsightsRestApiSessionData User { get; set; }

        [JsonProperty(PropertyName = "dependency")]
        public InsightsRestApiDependencyData Dependency { get; set; }

        [JsonProperty(PropertyName = "client")]
        public InsightsRestApiClientData Client { get; set; }

    }
}
