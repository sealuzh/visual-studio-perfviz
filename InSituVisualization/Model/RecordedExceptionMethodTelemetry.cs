using System;
using InSituVisualization.TelemetryCollector.DataCollection;

namespace InSituVisualization.Model
{
    public class RecordedExceptionMethodTelemetry : RecordedMethodTelemetry
    {
        public string ErrorText;

        public RecordedExceptionMethodTelemetry(string documentationCommentId, string id, DateTime timestamp, string name, string errorText) : base(documentationCommentId, id, timestamp, name)
        {
            ErrorText = errorText;
        }

        public static RecordedExceptionMethodTelemetry FromDataEntity(CollectedDataEntity dataEntity)
        {
            return new RecordedExceptionMethodTelemetry(
                dataEntity.DependencyData.Name, 
                dataEntity.Id, 
                dataEntity.Timestamp, 
                dataEntity.DependencyData.Type, 
                dataEntity.DependencyData.Data);
        }
    }
}
