using System;
using InSituVisualization.Annotations;
using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    internal class InSituLoopControlViewModel : ViewModelBase
    {
        private const int DefaultMaxLoopIterations = 100;
        private int _loopIterations;

        public InSituLoopControlViewModel(LoopPerformanceInfo loopPerformanceInfo)
        {
            LoopPerformanceInfo = loopPerformanceInfo;
            // initializing LoopIterations to Average
            _loopIterations = LoopPerformanceInfo.PredictedLoopIterations > 0 ? LoopPerformanceInfo.PredictedLoopIterations : LoopPerformanceInfo.AverageLoopIterations;

        }

        public LoopPerformanceInfo LoopPerformanceInfo { get; }

        /// <summary>
        /// Number of Iterations in the loop
        /// </summary>
        public int LoopIterations
        {
            get => _loopIterations;
            set
            {
                if (!SetProperty(ref _loopIterations, value))
                {
                    return;
                }
                // Updating Predicted Method Time
                LoopPerformanceInfo.PredictedLoopIterations = value;
            }
        }

        /// <summary>
        /// The Maximum number of iterations to make available via slider
        /// </summary>
        public int MaxLoopIterations
        {
            get
            {
                if (_loopIterations <= 0)
                {
                    return DefaultMaxLoopIterations;
                }
                return _loopIterations * 100;
            }
        }

        public TimeSpan ExecutionTime => LoopPerformanceInfo.ExecutionTime;
    }
}
