using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace InSituVisualization.Model
{
    /// <summary>
    /// Aggregated Average Data
    /// </summary>
    internal class AveragedMethodTelemetry : MethodTelemetry
    {
        public TimeSpan Duration { get; }
        public int MemberCount { get; }
        public int ExceptionCount { get; }
        public ConcurrentDictionary<string, RecordedDurationMethodTelemetry> TelemetryMembers { get; }
        public ConcurrentDictionary<string, RecordedExceptionMethodTelemetry> ExceptionMembers { get; }

        public AveragedMethodTelemetry(string documentationCommentId, ConcurrentDictionary<string, RecordedDurationMethodTelemetry> telemetryMembers, ConcurrentDictionary<string, RecordedExceptionMethodTelemetry> exceptionMembers) : base(documentationCommentId)
        {
            TelemetryMembers = telemetryMembers;
            ExceptionMembers = exceptionMembers;
            MemberCount = telemetryMembers.Count;
            ExceptionCount = exceptionMembers.Count;
            Duration = GetAverage();
        }

        public AveragedMethodTelemetry(string documentationCommentId, ConcurrentDictionary<string, RecordedDurationMethodTelemetry> telemetryMembers) : base(documentationCommentId)
        {
            TelemetryMembers = telemetryMembers;
            ExceptionMembers = null;
            MemberCount = telemetryMembers.Count;
            ExceptionCount = 0;
            Duration = GetAverage();
        }

        private TimeSpan GetAverage()
        {
            var timeList = new List<double>();

            foreach (var telemetry in TelemetryMembers.Values)
            {
                timeList.Add(telemetry.Duration);
            }
            return TelemetryMembers.Values.Count > 0 ? TimeSpan.FromMilliseconds(timeList.Average()) : TimeSpan.Zero;
        }
    }
}
