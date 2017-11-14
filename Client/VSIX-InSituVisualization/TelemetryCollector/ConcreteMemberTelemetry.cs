using System;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    /// <summary>
    /// Telemetry collected from single run
    /// </summary>
    internal class ConcreteMemberTelemetry : MemberTelemetry
    {
        public ConcreteMemberTelemetry(string id, DateTime timestamp, string memberName, TimeSpan duration) : base(memberName, duration)
        {
            Id = id;
            Timestamp = timestamp;
        }

        public string Id { get; }
        public DateTime Timestamp { get; }
    }
}
