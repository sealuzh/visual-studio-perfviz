using System;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    /// <summary>
    /// Telemetry collected from single run
    /// </summary>
    internal class ConcreteTelemetryMember : TelemetryMember
    {
        public ConcreteTelemetryMember(string id, DateTime timestamp, string memberName, int duration, string city, string completeAssemblyName, string nameSpace) : base(memberName, nameSpace, duration)
        {
            Id = id;
            Timestamp = timestamp;
            City = city;
            CompleteAssemblyName = completeAssemblyName;
        }

        public string Id { get; }
        public DateTime Timestamp { get; }
        public string City { get; }
        public string CompleteAssemblyName { get; }

    }
}
