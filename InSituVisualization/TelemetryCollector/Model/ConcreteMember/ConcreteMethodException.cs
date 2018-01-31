using System;

namespace InSituVisualization.TelemetryCollector.Model.ConcreteMember
{
    public class ConcreteMethodException : ConcreteMethod//, IConcreteMethod
    {
        public ConcreteMethodException(string documentationCommentId, string id, DateTime timestamp, string type) : base(documentationCommentId, id, timestamp, type)
        {

        }
    }
}
