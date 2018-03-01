using System.Collections.Concurrent;

namespace InSituVisualization.Model
{
    /// <summary>
    /// All Available Data of a Method
    /// This includes Telemetry
    /// This includes Predictions
    /// </summary>
    public class MethodPerformanceData
    {
        public ConcurrentBag<RecordedDurationMethodTelemetry> Durations { get; } = new ConcurrentBag<RecordedDurationMethodTelemetry>();
        public ConcurrentBag<RecordedExceptionMethodTelemetry> Exceptions { get; } = new ConcurrentBag<RecordedExceptionMethodTelemetry>();
    }
}
