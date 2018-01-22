using System;
using System.Collections.Generic;

namespace InSituVisualization.TelemetryCollector
{
    /// <summary>
    /// Aggregated Average Data
    /// </summary>
    internal class AveragedMethodTelemetry : MethodTelemetry
    {
        public TimeSpan Duration { get; }
        public int MemberCount { get; }
        public IDictionary<String, ConcreteMethodTelemetry> Members { get; }

        public AveragedMethodTelemetry(string documentationCommentId, TimeSpan duration, int memberCount, IDictionary<string, ConcreteMethodTelemetry> members)
            : base(documentationCommentId)
        {
            Duration = duration;
            MemberCount = memberCount;
            Members = members;
        }
    }
}
