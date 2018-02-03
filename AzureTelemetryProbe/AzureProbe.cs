using System;
using System.Collections.Concurrent;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
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

        /// <summary>
        /// This is the Method executed before the Method Body
        /// Important: Name must match <see cref="OnBeforeMethod"/>
        /// </summary>
        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void OnBeforeMethod(string documentationCommentId)
        {
            MethodStartDateTimes.TryAdd(documentationCommentId, DateTime.UtcNow);
        }

        /// <summary>
        /// This is the Method executed after the Method Body
        /// Important: Name must match <see cref="OnAfterMethod"/>
        /// </summary>
        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void OnAfterMethod(string documentationCommentId)
        {
            if (!MethodStartDateTimes.TryRemove(documentationCommentId, out var startDateTime))
            {
                return;
            }
            TelemetryClient.TrackDependency(documentationCommentId, "", startDateTime, DateTime.UtcNow - startDateTime, true);
        }

        /// <summary>
        /// This is the Method executed before exceptions
        /// Important: Name must match <see cref="OnException"/>
        /// </summary>
        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void OnException(string documentationCommentId)
        {
            var telemetry = new EventTelemetry("Exception")
            {
                Properties =
                {
                    {"Method", documentationCommentId}
                },
                Timestamp = DateTime.UtcNow
            };

            TelemetryClient.TrackEvent(telemetry);
        }

    }
}
