using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace InSituVisualization.Model
{
    public interface IMethodPerformanceData : INotifyPropertyChanged
    {
        ObservableCollection<RecordedExceptionMethodTelemetry> Exceptions { get; }
        ObservableCollection<RecordedExecutionTimeMethodTelemetry> ExecutionTimes { get; }
        IList<RecordedExecutionTimeMethodTelemetry> FilteredExecutionTimes { get; }
        TimeSpan MeanExecutionTime { get; }
        TimeSpan TotalExecutionTime { get; }
    }
}