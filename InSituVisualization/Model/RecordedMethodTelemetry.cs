using System;

namespace InSituVisualization.Model
{
    public class RecordedMethodTelemetry : MethodTelemetry
    {
        public string Id { get; }
        public DateTime Timestamp { get; }
        public string Name { get;  } 

        public RecordedMethodTelemetry(string documentationCommentId, string id, DateTime timestamp, string name) : base(documentationCommentId)
        {
            Id = id;
            Timestamp = timestamp;
            Name = name;
        }

    }
}
