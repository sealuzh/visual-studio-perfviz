using System;
using InSituVisualization.TelemetryCollector.DataCollection;

namespace InSituVisualization.Model
{
    public class ConcreteMethodException : ConcreteMethod
    {
        public string ErrorText;

        public ConcreteMethodException(string documentationCommentId, string id, DateTime timestamp, string name, string errorText) : base(documentationCommentId, id, timestamp, name)
        {
            ErrorText = errorText;
        }

        public static ConcreteMethodException FromDataEntity(CollectedDataEntity dataEntity)
        {
            return new ConcreteMethodException(
                dataEntity.DependencyData.Name, 
                dataEntity.Id, 
                dataEntity.Timestamp, 
                dataEntity.DependencyData.Type, 
                dataEntity.DependencyData.Data);
        }
    }
}
