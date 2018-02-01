using System;

namespace InSituVisualization.TelemetryCollector.Model.ConcreteMember
{
    public class ConcreteMethodException : ConcreteMethod
    {
        public string ErrorText;

        public ConcreteMethodException(string documentationCommentId, string id, DateTime timestamp, string type, string errorText) : base(documentationCommentId, id, timestamp, type)
        {
            ErrorText = errorText;
        }
    }
}
