using System;

namespace InSituVisualization.Model
{
    /// <inheritdoc />
    /// <summary>
    /// Telemetry collected about a specific method in a single run
    /// </summary>
    public class RecordedExecutionTimeMethodTelemetry : RecordedMethodTelemetry
    {
        public int Duration { get; }

        public RecordedExecutionTimeMethodTelemetry(string documentationCommentId, string id, DateTime timestamp, int duration, IClientData clientData) : base(documentationCommentId, id, timestamp, clientData)
        {
            Duration = duration;
        }
    }
}
