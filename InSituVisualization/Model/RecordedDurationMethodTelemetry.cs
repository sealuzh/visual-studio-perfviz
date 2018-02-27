using System;
using InSituVisualization.TelemetryCollector.DataCollection;

namespace InSituVisualization.Model
{
    /// <summary>
    /// Telemetry collected about a specific method in a single run
    /// </summary>
    public class RecordedDurationMethodTelemetry : RecordedMethodTelemetry
    {
        public int Duration { get; }
        public string City { get; }

        public RecordedDurationMethodTelemetry(string documentationCommentId, string id, DateTime timestamp, string name, int duration, string city) : base(documentationCommentId, id, timestamp, name)
        {
            Duration = duration;
            City = city;
        }

        public static RecordedDurationMethodTelemetry FromDataEntity(CollectedDataEntity dataEntity)
        {
            return new RecordedDurationMethodTelemetry(
                dataEntity.DependencyData.Name,
                dataEntity.Id,
                dataEntity.Timestamp, 
                dataEntity.DependencyData.Type, 
                dataEntity.DependencyData.Duration, 
                dataEntity.ClientData.City);
        }

    }
}
