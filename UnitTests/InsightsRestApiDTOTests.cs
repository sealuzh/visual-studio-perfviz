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
        private string _telemetryJson;

        [TestInitialize]
        public void Init()
        {
            _telemetryJson = System.IO.File.ReadAllText(@"InsightsApiResponse.json");
        }


        [TestMethod]
        public void TestInsightsRestApiResponse()
        {
            var telemetryData = JsonConvert.DeserializeObject<InsightsRestApiResponse>(_telemetryJson);
            Assert.AreEqual(500, telemetryData.Value.Count);
            var firstValue = telemetryData.Value.First();
            Assert.AreEqual("d5d41320-1ad2-11e8-b049-1129a19ac613", firstValue.Id);
            Assert.AreEqual("dependency", firstValue.Type);
            Assert.AreEqual(1, firstValue.Count);
            Assert.AreEqual(DateTimeOffset.Parse("2018-02-26T08:55:46.058Z"), firstValue.Timestamp);
            Assert.IsNotNull(firstValue.Dependency);
            Assert.IsNotNull(firstValue.Client);
        }

        [TestMethod]
        public void TestInsightsRestApiDependencyData()
        {
            var telemetryData = JsonConvert.DeserializeObject<InsightsRestApiResponse>(_telemetryJson);
            var dependency = telemetryData.Value.First().Dependency;
            Assert.AreEqual("-target-", dependency.Target);
            Assert.AreEqual("-data-", dependency.Data);
            Assert.AreEqual(true, dependency.Success);
            Assert.AreEqual(12, dependency.Duration);
            Assert.AreEqual("0", dependency.ResultCode);
            Assert.AreEqual("telemetry", dependency.Type);
            Assert.AreEqual("M:Microsoft.eShopWeb.Services.BasketViewModelService.<CreateViewModelFromBasket>b__5_0(Microsoft.eShopWeb.ApplicationCore.Entities.BasketItem)", dependency.Name);
            Assert.AreEqual("VhFQCpzLn2g=", dependency.Id);
        }

        [TestMethod]
        public void TestInsightsRestApiClientData()
        {
            var telemetryData = JsonConvert.DeserializeObject<InsightsRestApiResponse>(_telemetryJson);
            var client = telemetryData.Value.First().Client;
            Assert.AreEqual("PC", client.Type);
            Assert.AreEqual("0.0.0.0", client.Ip);
            Assert.AreEqual("Amsterdam", client.City);
            Assert.AreEqual("North Holland", client.StateOrProvince);
            Assert.AreEqual("Netherlands", client.CountryOrRegion);
        }
    }
}
