using System;
using System.Collections.Concurrent;
using System.Linq;
using InSituVisualization.Utils;

namespace InSituVisualization.Model
{
    /// <summary>
    /// All Available Data of a Method
    /// This includes Telemetry
    /// This includes Predictions
    /// </summary>
    public class MethodPerformanceData : ModelBase
    {
        private TimeSpan? _meanExecutionTime;
        private TimeSpan? _totalExecutionTime;

        public ConcurrentBag<RecordedDurationMethodTelemetry> Durations { get; } = new ConcurrentBag<RecordedDurationMethodTelemetry>();
        public ConcurrentBag<RecordedExceptionMethodTelemetry> Exceptions { get; } = new ConcurrentBag<RecordedExceptionMethodTelemetry>();

        public TimeSpan MeanExecutionTime
        {
            get => _meanExecutionTime ?? GetAverageDuration();
            set => SetProperty(ref _meanExecutionTime, value);
        }

        public TimeSpan TotalExecutionTime
        {
            get => _totalExecutionTime ?? Durations.Sum(p => TimeSpan.FromMilliseconds(p.Duration));
            set => SetProperty(ref _totalExecutionTime, value);
        }

        private TimeSpan GetAverageDuration()
        {
            if (Durations.Count <= 0)
            {
                return TimeSpan.Zero;
            }
            return TimeSpan.FromMilliseconds(Durations.Select(telemetry => telemetry.Duration).Average());
        }

    }
}
