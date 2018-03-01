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
        public BundleMethodTelemetry(string documentationCommentId) : base(documentationCommentId)
        {
        }

        public ConcurrentBag<RecordedDurationMethodTelemetry> Durations { get; } = new ConcurrentBag<RecordedDurationMethodTelemetry>();
        public ConcurrentBag<RecordedExceptionMethodTelemetry> Exceptions { get; } = new ConcurrentBag<RecordedExceptionMethodTelemetry>();

        public TimeSpan GetAverageDuration()
        {
            if (Durations.Count <= 0)
            {
                return TimeSpan.Zero;
            }
            return TimeSpan.FromMilliseconds(Durations.Select(telemetry => telemetry.Duration).Average());
        }
    }
}
