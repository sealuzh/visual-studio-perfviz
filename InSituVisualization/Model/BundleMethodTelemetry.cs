using System;
using System.Collections.Concurrent;
using System.Linq;

namespace InSituVisualization.Model
{
    /// <summary>
    /// All Available Telemetry Data of a Method
    /// </summary>
    internal class BundleMethodTelemetry : MethodTelemetry
    {
        public ConcurrentDictionary<string, RecordedDurationMethodTelemetry> Durations { get; }
        public ConcurrentDictionary<string, RecordedExceptionMethodTelemetry> Exceptions { get; }

        public BundleMethodTelemetry(string documentationCommentId, ConcurrentDictionary<string, RecordedDurationMethodTelemetry> durations, ConcurrentDictionary<string, RecordedExceptionMethodTelemetry> exceptions) : base(documentationCommentId)
        {
            Durations = durations;
            Exceptions = exceptions;
        }

        public TimeSpan GetAverageDuration()
        {
            if (Durations.Values.Count <= 0)
            {
                return TimeSpan.Zero;
            }
            return TimeSpan.FromMilliseconds(Durations.Values.Select(telemetry => telemetry.Duration).Average());
        }
    }
}
