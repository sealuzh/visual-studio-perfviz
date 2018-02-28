using System;
using System.Collections.Generic;
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

        public IList<RecordedDurationMethodTelemetry> Durations { get; } = new List<RecordedDurationMethodTelemetry>();
        public IList<RecordedExceptionMethodTelemetry> Exceptions { get; } = new List<RecordedExceptionMethodTelemetry>();

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
