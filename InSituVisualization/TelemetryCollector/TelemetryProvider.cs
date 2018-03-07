using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.Model;

namespace InSituVisualization.TelemetryCollector
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class TelemetryProvider : ITelemetryProvider
    {
        private static readonly TimeSpan ThrottleTimeSpan = TimeSpan.FromMinutes(1);
        private readonly TelemetryCollectorRegistry _telemetryCollectorRegistry;

        private DateTime _lastUpdate = default(DateTime);

        public TelemetryProvider(TelemetryCollectorRegistry telemetryCollectorRegistry)
        {
            _telemetryCollectorRegistry = telemetryCollectorRegistry;
        }

        private HashSet<string> AcknowledgedTelemetryIds { get; } = new HashSet<string>();
        private Dictionary<string, MethodPerformanceData> TelemetryByDocumentationCommentId { get; } = new Dictionary<string, MethodPerformanceData>();

        public async Task<MethodPerformanceData> GetTelemetryDataAsync(string documentationCommentId)
        {
            await UpdateTelemetryDataAsync();
            return TelemetryByDocumentationCommentId.TryGetValue(documentationCommentId, out var methodTelemetry) ? methodTelemetry : null;
        }

        private async Task UpdateTelemetryDataAsync()
        {
            if (_lastUpdate + ThrottleTimeSpan > DateTime.Now)
            {
                return;
            }
            _lastUpdate = DateTime.Now;

            foreach (var telemetryCollector in _telemetryCollectorRegistry.TelemetryCollectors)
            {
                var recordedMethodTelemetries = await telemetryCollector.GetTelemetryAsync();
                foreach (var methodTelemetry in recordedMethodTelemetries)
                {
                    if (!AcknowledgedTelemetryIds.Add(methodTelemetry.Id) || string.IsNullOrWhiteSpace(methodTelemetry.DocumentationCommentId))
                    {
                        // Telemetry is already known or no documentationCommentId
                        continue;
                    }

                    TelemetryByDocumentationCommentId.TryGetValue(methodTelemetry.DocumentationCommentId, out var performanceData);
                    if (performanceData == null)
                    {
                        performanceData = new MethodPerformanceData();
                        TelemetryByDocumentationCommentId.Add(methodTelemetry.DocumentationCommentId, performanceData);
                    }

                    switch (methodTelemetry)
                    {
                        case RecordedExecutionTimeMethodTelemetry recordedDurationMethodTelemetry:
                            performanceData.ExecutionTimes.Add(recordedDurationMethodTelemetry);
                            break;
                        case RecordedExceptionMethodTelemetry exceptionTelemetry:
                            performanceData.Exceptions.Add(exceptionTelemetry);
                            break;
                    }
                }
            }
        }
    }
}
