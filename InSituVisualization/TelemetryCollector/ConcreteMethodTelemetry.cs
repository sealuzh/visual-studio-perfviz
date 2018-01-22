using System;

namespace InSituVisualization.TelemetryCollector
{
    /// <summary>
    /// Telemetry collected about a specific method in a single run
    /// </summary>
    internal class ConcreteMethodTelemetry : MethodTelemetry
    {
        public ConcreteMethodTelemetry(string documentationCommentId, string id, DateTime timestamp, int duration, string city)
            : base(documentationCommentId)
        {
            Id = id;
            Timestamp = timestamp;
            Duration = duration;
            City = city;
        }

        public string Id { get; }
        public DateTime Timestamp { get; }
        public int Duration { get; }
        public string City { get; }
    }
}
