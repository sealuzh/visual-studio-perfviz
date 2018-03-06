namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public class ClientData
    {
        public ClientData(dynamic inputTelemetryData)
        {
            Model = (string)inputTelemetryData.client.model;
            Os = (string)inputTelemetryData.client.os;
            Type = (string)inputTelemetryData.client.type;
            Browser = (string)inputTelemetryData.client.browser;
            Ip = (string)inputTelemetryData.client.ip;
            City = (string)inputTelemetryData.client.city;
            StateOrProvince = (string)inputTelemetryData.client.stateOrProvince;
            CountryOrRegion = (string)inputTelemetryData.client.countryOrRegion;
        }

        /// <summary>
        /// Unknown
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// OS running on calling system
        /// </summary>
        public string Os { get; }

        /// <summary>
        /// PC / Mac
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Name of browser
        /// </summary>
        public string Browser { get; }

        /// <summary>
        /// IP Adress
        /// </summary>
        public string Ip { get; }

        /// <summary>
        /// City Name
        /// </summary>
        public string City { get; }

        /// <summary>
        /// Region city is in
        /// </summary>
        public string StateOrProvince { get; }

        /// <summary>
        /// Country of state
        /// </summary>
        public string CountryOrRegion { get; }
    }
}
