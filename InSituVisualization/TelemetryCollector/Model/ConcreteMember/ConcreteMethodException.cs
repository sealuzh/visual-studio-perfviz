using System;

namespace InSituVisualization.TelemetryCollector.Model.ConcreteMember
{
    public class ConcreteMethodException : ConcreteMethod
    {
        public string ErrorText;

        public ConcreteMethodException(string documentationCommentId, string id, DateTime timestamp, string name, string errorText) : base(documentationCommentId, id, timestamp, name)
        {
            ErrorText = errorText;
        }
    }
}
