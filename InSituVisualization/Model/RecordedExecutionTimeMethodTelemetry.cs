using System;

namespace InSituVisualization.Model
{
    /// <inheritdoc />
    /// <summary>
    /// Telemetry collected about a specific method in a single run
    /// </summary>
    public class RecordedExecutionTimeMethodTelemetry : RecordedMethodTelemetry
    {
        public TimeSpan Duration { get; }

        public RecordedExecutionTimeMethodTelemetry(string documentationCommentId, string id, DateTime timestamp, TimeSpan duration, IClientData clientData) : base(documentationCommentId, id, timestamp, clientData)
        {
            Duration = duration;
        }
    }
}
