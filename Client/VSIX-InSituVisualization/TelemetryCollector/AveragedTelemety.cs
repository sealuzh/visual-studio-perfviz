using System;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    /// <summary>
    /// Aggregated Average Data
    /// </summary>
    internal class AveragedMethodTelemetry : MethodTelemetry
    {
        public TimeSpan Duration { get; }
        public int MemberCount { get; }

        public AveragedMethodTelemetry(string documentationCommentId, TimeSpan duration, int memberCount)
            : base(documentationCommentId)
        {
            Duration = duration;
            MemberCount = memberCount;
        }
    }
}
