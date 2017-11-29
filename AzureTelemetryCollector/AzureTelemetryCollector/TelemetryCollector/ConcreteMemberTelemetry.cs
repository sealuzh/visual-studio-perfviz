using System;

namespace AzureTelemetryCollector.TelemetryCollector
{
    /// <summary>
    /// Telemetry collected from single run
    /// </summary>
    internal class ConcreteMemberTelemetry : MemberTelemetry
    {
        public ConcreteMemberTelemetry(string id, DateTime timestamp, string memberName, TimeSpan duration, String city) : base(memberName, duration)
        {
            Id = id;
            Timestamp = timestamp;
            City = city;
        }

        public string Id { get; }
        public DateTime Timestamp { get; }
        public string City { get; }
    }
}
