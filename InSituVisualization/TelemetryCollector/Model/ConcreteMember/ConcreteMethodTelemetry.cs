using System;

namespace InSituVisualization.TelemetryCollector.Model.ConcreteMember
{
    /// <summary>
    /// Telemetry collected about a specific method in a single run
    /// </summary>
    public class ConcreteMethodTelemetry : ConcreteMethod//, IConcreteMethod
    {
        public int Duration { get; }
        public string City { get; }

        public ConcreteMethodTelemetry(string documentationCommentId, string id, DateTime timestamp, string type, int duration, string city) : base(documentationCommentId, id, timestamp, type)
        {
            Duration = duration;
            City = city;
        }

    }
}
