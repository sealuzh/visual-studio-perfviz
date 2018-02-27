using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace InSituVisualization.Model
{
    /// <summary>
    /// Aggregated Average Data
    /// </summary>
    internal class AveragedMethod : Method
    {
        public TimeSpan Duration { get; }
        public int MemberCount { get; }
        public int ExceptionCount { get; }
        public ConcurrentDictionary<string, ConcreteMethodTelemetry> TelemetryMembers { get; }
        public ConcurrentDictionary<string, ConcreteMethodException> ExceptionMembers { get; }

        public AveragedMethod(string documentationCommentId, ConcurrentDictionary<string, ConcreteMethodTelemetry> telemetryMembers, ConcurrentDictionary<string, ConcreteMethodException> exceptionMembers) : base(documentationCommentId)
        {
            TelemetryMembers = telemetryMembers;
            ExceptionMembers = exceptionMembers;
            MemberCount = telemetryMembers.Count;
            ExceptionCount = exceptionMembers.Count;
            Duration = GetAverage();
        }

        public AveragedMethod(string documentationCommentId, ConcurrentDictionary<string, ConcreteMethodTelemetry> telemetryMembers) : base(documentationCommentId)
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
