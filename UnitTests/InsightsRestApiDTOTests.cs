using System;
using System.Linq;
using InSituVisualization.TelemetryCollector.DataCollection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTests
{
    [TestClass]
    public class InsightsRestApiDtoTests
    {
        public string TelemetryJson;

        [TestInitialize]
        public void Init()
        {
            TelemetryJson = System.IO.File.ReadAllText(@"InsightsApiResponse.json");
        }


        [TestMethod]
        public void TestInsightsRestApiResponse()
        {
            var telemetryData = JsonConvert.DeserializeObject<InsightsRestApiResponse>(TelemetryJson);
            Assert.AreEqual(500, telemetryData.Value.Count);
            var firstValue = telemetryData.Value.First();
            Assert.AreEqual("d5d41320-1ad2-11e8-b049-1129a19ac613", firstValue.Id);
            Assert.AreEqual("dependency", firstValue.Type);
            Assert.AreEqual(1, firstValue.Count);
            Assert.AreEqual(DateTimeOffset.Parse("2018-02-26T08:55:46.058Z"), firstValue.Timestamp);
            Assert.IsNotNull(firstValue.Dependency);
            Assert.IsNotNull(firstValue.Client);
        }
    }
}
