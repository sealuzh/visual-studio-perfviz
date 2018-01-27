using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace AzureTelemetryProbe
{
    // ReSharper disable once UnusedMember.Global Justification: Reflection
    public class AzureProbe
    {

        private static readonly ConcurrentDictionary<string, DateTime> MethodStartDateTimes = new ConcurrentDictionary<string, DateTime>();

        static AzureProbe()
        {
            TelemetryConfiguration.Active.InstrumentationKey = "09725176-e81e-436d-bf21-958cad8d3a5a";
        }

        private static TelemetryClient TelemetryClient { get; } = new TelemetryClient { InstrumentationKey = TelemetryConfiguration.Active.InstrumentationKey };

        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void PreMethodExecutionHook(string documentationCommentId)
        {
            MethodStartDateTimes.TryAdd(documentationCommentId, DateTime.UtcNow);
            Trace.WriteLine($"{DateTime.UtcNow} - {documentationCommentId}. Started");
        }

        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void PostMethodExecutionHook(string documentationCommentId)
        {
            if (!MethodStartDateTimes.TryRemove(documentationCommentId, out var startDateTime))
            {
                return;
            }
            var duration = DateTime.UtcNow - startDateTime;
            Trace.WriteLine($"{DateTime.UtcNow} - {documentationCommentId}. Duration: {duration.Milliseconds} ms");
            TelemetryClient.TrackDependency(documentationCommentId, "", startDateTime, duration, true);
        }

    }
}
