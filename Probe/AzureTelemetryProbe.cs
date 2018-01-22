using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.ApplicationInsights;

namespace Probe
{
    public class AzureTelemetryProbe
    {
        private static readonly ConcurrentDictionary<string, DateTime> MethodStartDateTimes = new ConcurrentDictionary<string, DateTime>();

        public static void PreMethodBodyHook(string documentationCommentId)
        {
            MethodStartDateTimes.TryAdd(documentationCommentId, DateTime.UtcNow);
            Trace.WriteLine($"{documentationCommentId}. Start");
        }
        public static void PostMethodBodyHook(string documentationCommentId)
        {
            if (!MethodStartDateTimes.TryRemove(documentationCommentId, out var startDateTime))
            {
                return;
            }
            var duration = DateTime.UtcNow - startDateTime;
            Trace.WriteLine($"{documentationCommentId}. Duration: {duration.Milliseconds} ms");
        }

        //tracepath, type not important and can be neglected.
        public static void SendTelemetryToAzure(DateTime start, string type, string methodName, string tracePath)
        {
            TimeSpan duration = DateTime.UtcNow - start;
            TelemetryClient telemetryClient = new TelemetryClient();
            telemetryClient.TrackDependency(type, methodName, tracePath, "InsightsTest", start, duration, "0", true);
        }

    }
}
