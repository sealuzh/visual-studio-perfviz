using System;

namespace InSituVisualization.TelemetryCollector.Model.ConcreteMember
{
    public class ConcreteMethod : Method
    {
        public string Id { get; }
        public DateTime Timestamp { get; }
        public string Type { get;  } 

        public ConcreteMethod(string documentationCommentId, string id, DateTime timestamp, string type) : base(documentationCommentId)
        {
            Id = id;
            Timestamp = timestamp;
            Type = type;
        }

    }
}
