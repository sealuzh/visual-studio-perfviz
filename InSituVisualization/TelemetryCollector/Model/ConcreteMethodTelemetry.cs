using System;

namespace InSituVisualization.TelemetryCollector.Model
{
    /// <summary>
    /// Telemetry collected about a specific method in a single run
    /// </summary>
    public class ConcreteMethodTelemetry : ConcreteMethod//, IConcreteMethod
    {
        public int Duration { get; }
        public string City { get; }

        public ConcreteMethodTelemetry(string documentationCommentId, string id, DateTime timestamp, string name, int duration, string city) : base(documentationCommentId, id, timestamp, name)
        {
            Duration = duration;
            City = city;
        }

    }
}
