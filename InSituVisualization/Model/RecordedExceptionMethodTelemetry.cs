using System;

namespace InSituVisualization.Model
{
    public class RecordedExceptionMethodTelemetry : RecordedMethodTelemetry
    {
        public RecordedExceptionMethodTelemetry(string documentationCommentId, string id, DateTime timestamp, string errorText, IClientData clientData) : base(documentationCommentId, id, timestamp, clientData)
        {
            ErrorText = errorText;
        }

        public string ErrorText { get; }
    }
}
