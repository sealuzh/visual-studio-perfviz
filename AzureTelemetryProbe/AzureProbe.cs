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

        private static ConcurrentDictionary<string, DateTime> _methodStartDateTimes;
        private const string InstrumentationKey = "2f2bd3a7-8234-4722-ad09-6add2f1aeaaf";
        private static TelemetryClient _telemetryClient;

        private static void Init()
        {
           _methodStartDateTimes = new ConcurrentDictionary<string, DateTime>();
            TelemetryConfiguration.Active.InstrumentationKey = InstrumentationKey;
            _telemetryClient = new TelemetryClient
            {
                InstrumentationKey = InstrumentationKey
            };
        }

        /// <summary>
        /// This is the Method executed before the Method Body
        /// Important: Name must match <see cref="OnBeforeMethod"/>
        /// </summary>
        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void OnBeforeMethod(string documentationCommentId)
        {
            if (_telemetryClient == null)
            {
                Init();
            }
            _methodStartDateTimes?.TryAdd(documentationCommentId, DateTime.UtcNow);
        }

        /// <summary>
        /// This is the Method executed after the Method Body
        /// Important: Name must match <see cref="OnAfterMethod"/>
        /// </summary>
        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void OnAfterMethod(string documentationCommentId)
        {
            if (!_methodStartDateTimes.TryRemove(documentationCommentId, out var startDateTime))
            {
                return;
            }
            _telemetryClient.TrackDependency("telemetry", "-target-", documentationCommentId, "-data-", startDateTime, DateTime.UtcNow - startDateTime, "0", true);
            _telemetryClient.Flush();
        }

        /// <summary>
        /// This is the Method executed before exceptions
        /// Important: Name must match <see cref="OnException"/>
        /// </summary>
        // ReSharper disable once UnusedMember.Global Justification: Reflection
        public static void OnException(string documentationCommentId)
        {
            if (!_methodStartDateTimes.TryRemove(documentationCommentId, out var startDateTime))
            {
                return;
            }

            _telemetryClient.TrackDependency("exception", "-target-", documentationCommentId, "-data-", DateTime.UtcNow, TimeSpan.Zero, "0", true);
            _telemetryClient.Flush();
            Console.WriteLine("Just sent data.");

            //var telemetry = new EventTelemetry("Exception")
            //{
            //    Properties =
            //    {
            //        {"Method", documentationCommentId}
            //    },
            //    Timestamp = DateTime.UtcNow
            //};

            //_telemetryClient?.TrackEvent(telemetry);
        }

    }
}
