using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public class InsightsRestApiDependencyData
    {
        /// <summary>
        /// Method Name of method that called
        /// </summary>
        [JsonProperty(PropertyName = "target")]
        public string Target { get; set; }

        /// <summary>
        /// Errormessage, other data.
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        /// <summary>
        /// Whether the call was successfull
        /// </summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        /// <summary>
        /// Duration of the method execution in ms
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        [JsonProperty(PropertyName = "performanceBucket")]
        public string PerformanceBucket { get; set; }

        /// <summary>
        /// Returned result code of the function (should always be 0)
        /// </summary>
        [JsonProperty(PropertyName = "resultCode")]
        public string ResultCode { get; set; }

        /// <summary>
        /// Exception / Telemetry
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }


        /// <summary>
        /// Methodname
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

    }
}
