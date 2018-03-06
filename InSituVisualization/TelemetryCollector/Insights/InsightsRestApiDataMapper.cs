using System.Collections.Generic;
using InSituVisualization.Model;

namespace InSituVisualization.TelemetryCollector.Insights
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IoC
    internal class InsightsRestApiDataMapper
    {
        public IList<RecordedMethodTelemetry> GetMethodTelemetry(InsightsRestApiResponse insightsRestApiResponse)
        {
            var list = new List<RecordedMethodTelemetry>();
            foreach (var insightsRestApiResponseValue in insightsRestApiResponse.Value)
            {
                if (string.IsNullOrWhiteSpace(insightsRestApiResponseValue.Id) || string.IsNullOrWhiteSpace(insightsRestApiResponseValue.Dependency.Name))
                {
                    continue;
                }

                switch (insightsRestApiResponseValue.Dependency.Type)
                {
                    case "telemetry":
                        list.Add(new RecordedExecutionTimeMethodTelemetry(
                            insightsRestApiResponseValue.Dependency.Name,
                            insightsRestApiResponseValue.Id,
                            insightsRestApiResponseValue.Timestamp,
                            insightsRestApiResponseValue.Dependency.Duration,
                            insightsRestApiResponseValue.Client));
                        break;
                    case "exception":
                        list.Add(new RecordedExceptionMethodTelemetry(
                            insightsRestApiResponseValue.Dependency.Name,
                            insightsRestApiResponseValue.Id,
                            insightsRestApiResponseValue.Timestamp,
                            insightsRestApiResponseValue.Dependency.Data,
                            insightsRestApiResponseValue.Client));
                        break;
                }
            }
            return list;
        }
    }
}
