using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InSituVisualization.Filter;
using InSituVisualization.Utils;

namespace InSituVisualization.Model
{
    /// <summary>
    /// All Available Data of a Method
    /// This includes Telemetry
    /// This includes Predictions
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global, Justification: IoC
    public class MethodPerformanceData : ModelBase, IMethodPerformanceData
    {
        private readonly IFilterController _filterController;

        public MethodPerformanceData(IFilterController filterController)
        {
            _filterController = filterController;
            ExecutionTimes.CollectionChanged += (s, e) => OnFilterRenew();
            filterController.FiltersChanged += (s, e) => OnFilterRenew();
        }

        private void OnFilterRenew()
        {
            OnPropertyChanged(nameof(FilteredExecutionTimes));
            OnPropertyChanged(nameof(MeanExecutionTime));
            OnPropertyChanged(nameof(TotalExecutionTime));
        }

        public ObservableCollection<RecordedExecutionTimeMethodTelemetry> ExecutionTimes { get; } = new ObservableCollection<RecordedExecutionTimeMethodTelemetry>();
        public IList<RecordedExecutionTimeMethodTelemetry> FilteredExecutionTimes => _filterController.ApplyFilters(ExecutionTimes);

        public ObservableCollection<RecordedExceptionMethodTelemetry> Exceptions { get; } = new ObservableCollection<RecordedExceptionMethodTelemetry>();

        public TimeSpan MeanExecutionTime
        {
            get
            {
                if (!FilteredExecutionTimes.Any())
                {
                    return TimeSpan.Zero;
                }
                return TimeSpan.FromMilliseconds(ExecutionTimes.Select(telemetry => telemetry.Duration).Average());
            }
        }

        public TimeSpan TotalExecutionTime => FilteredExecutionTimes.Sum(p => TimeSpan.FromMilliseconds(p.Duration));
    }
}
