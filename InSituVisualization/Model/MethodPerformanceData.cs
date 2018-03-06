using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
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

        public MethodPerformanceData()
        {
            FilteredExecutionTimes = CollectionViewSource.GetDefaultView(ExecutionTimes);
            FilteredExecutionTimes.Filter += p => FilterExecutionTimes((RecordedExecutionTimeMethodTelemetry)p);
            ExecutionTimes.CollectionChanged += (s, e) => FilteredExecutionTimes.Refresh();
        }

        private bool FilterExecutionTimes(RecordedExecutionTimeMethodTelemetry recordedExecutionTimeMethodTelemetry)
        {
            return true;
        }

        public ObservableCollection<RecordedExecutionTimeMethodTelemetry> ExecutionTimes { get; } = new ObservableCollection<RecordedExecutionTimeMethodTelemetry>();
        public ICollectionView FilteredExecutionTimes { get; }
        public ObservableCollection<RecordedExceptionMethodTelemetry> Exceptions { get; } = new ObservableCollection<RecordedExceptionMethodTelemetry>();

        public TimeSpan MeanExecutionTime
        {
            get => _meanExecutionTime ?? GetAverageExecutionTime();
            set => SetProperty(ref _meanExecutionTime, value);
        }

        public TimeSpan TotalExecutionTime
        {
            get => _totalExecutionTime ?? ExecutionTimes.Sum(p => TimeSpan.FromMilliseconds(p.Duration));
            set => SetProperty(ref _totalExecutionTime, value);
        }

        private TimeSpan GetAverageExecutionTime()
        {
            if (ExecutionTimes.Count <= 0)
            {
                return TimeSpan.Zero;
            }
            return TimeSpan.FromMilliseconds(ExecutionTimes.Select(telemetry => telemetry.Duration).Average());
        }

    }
}
