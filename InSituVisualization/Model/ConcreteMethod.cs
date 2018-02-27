using System;

namespace InSituVisualization.Model
{
    public class ConcreteMethod : Method
    {
        public string Id { get; }
        public DateTime Timestamp { get; }
        public string Name { get;  } 

        public ConcreteMethod(string documentationCommentId, string id, DateTime timestamp, string name) : base(documentationCommentId)
        {
            Id = id;
            Timestamp = timestamp;
            Name = name;
        }

    }
}
