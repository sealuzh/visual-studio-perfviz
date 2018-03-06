using InSituVisualization.Model;
using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public class InsightsRestApiClientData : IClientData
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        /// <summary>
        /// OS running on calling system
        /// </summary>
        [JsonProperty(PropertyName = "os")]
        public string Os { get; set; }

        /// <summary>
        /// PC / Mac
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Name of browser
        /// </summary>
        [JsonProperty(PropertyName = "browser")]
        public string Browser { get; set; }

        /// <summary>
        /// IP Adress
        /// </summary>
        [JsonProperty(PropertyName = "ip")]
        public string Ip { get; set; }

        /// <summary>
        /// City Name
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// Region city is in
        /// </summary>
        [JsonProperty(PropertyName = "stateOrProvince")]
        public string StateOrProvince { get; set; }

        /// <summary>
        /// Country of state
        /// </summary>
        [JsonProperty(PropertyName = "countryOrRegion")]
        public string CountryOrRegion { get; set; }
    }
}
