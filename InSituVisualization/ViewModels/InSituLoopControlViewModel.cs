using System;
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
            _loopIterations = LoopPerformanceInfo.AverageLoopIterations;
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
                LoopPerformanceInfo.PredictExecutionTime();
            }
        }

        /// <summary>
        /// The Maximum number of iterations to make available via slider
        /// </summary>
        public int MaxLoopIterations
        {
            get
            {
                if (LoopPerformanceInfo.AverageLoopIterations <= 0)
                {
                    return DefaultMaxLoopIterations;
                }
                return LoopPerformanceInfo.AverageLoopIterations * 100;
            }
        }

        public TimeSpan ExecutionTime => LoopPerformanceInfo.ExecutionTime;
    }
}
