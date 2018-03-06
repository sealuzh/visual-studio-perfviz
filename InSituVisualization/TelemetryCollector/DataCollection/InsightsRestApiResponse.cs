using System.Collections.Generic;
using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public class InsightsRestApiResponse
    {
        // @odata.context

        // @ai.messages

        /// <summary>
        /// Value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public List<InsightsRestApiResponseValue> Value { get; set; }
    }
}
