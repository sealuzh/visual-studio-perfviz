using System;

namespace InSituVisualization.Model
{
    public abstract class PerformanceInfo : ModelBase
    {
        private TimeSpan _predictedExecutionTime;
        private bool _hasChanged;

        public TimeSpan ExecutionTime => HasPrediction ? PredictedExecutionTime : AverageExecutionTime;

        // ReSharper disable once MemberCanBePrivate.Global Justification: WPF Binding
        public bool HasPrediction => PredictedExecutionTime != default(TimeSpan);

        /// <summary>
        /// Are there Changes applied in the IDE
        /// The collected data will not apply anymore and output should resort to prediction
        /// </summary>
        public bool HasChanged
        {
            get => _hasChanged;
            set => SetProperty(ref _hasChanged, value);
        }

        protected abstract TimeSpan AverageExecutionTime { get; }

        /// <summary>
        /// Only to be set by subclasses/>
        /// </summary>
        protected TimeSpan PredictedExecutionTime
        {
            get => _predictedExecutionTime;
            set
            {
                if (!SetProperty(ref _predictedExecutionTime, value))
                {
                    return;
                }
                OnPropertyChanged(nameof(HasPrediction));
                OnPropertyChanged(nameof(ExecutionTime));
            }
        }
    }
}
