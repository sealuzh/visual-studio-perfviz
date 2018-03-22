using System;
using System.Collections.Generic;
using System.Linq;
using InSituVisualization.Model;
using InSituVisualization.TelemetryCollector.Insights;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class InsightsRestApiDataMapperTests
    {
        [TestMethod]
        public void InsightsRestApiResponseTest()
        {
            var insightsMapper = new InsightsRestApiDataMapper();

            var response = new InsightsRestApiResponse
            {
                Value = new List<InsightsRestApiResponseValue>()
            };

            var value = new InsightsRestApiResponseValue()
            {
                Id = "SomeOtherIdHere"
            };

            response.Value.Add(value);
            value.Dependency = new InsightsRestApiDependencyData
            {
                Id = "SomeIdHere",
                Name = "Test",
                Type = "telemetry",
                Duration = 10
            };

            var telemetry = insightsMapper.GetMethodTelemetry(response).First();

            Assert.AreEqual("Test", telemetry.DocumentationCommentId);
            Assert.AreEqual("SomeOtherIdHere", telemetry.Id);
            Assert.AreEqual(TimeSpan.FromMilliseconds(10), ((RecordedExecutionTimeMethodTelemetry) telemetry).Duration);
        }
    }
}
