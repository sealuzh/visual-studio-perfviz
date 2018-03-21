using System;
using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    internal class InSituLoopPerformanceInfoControlViewModel : ViewModelBase
    {
        private const int DefaultMaxLoopIterations = 100;
        private int _loopIterations;

        public InSituLoopPerformanceInfoControlViewModel(LoopPerformanceInfo loopPerformanceInfo)
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
                var oldExecutionTime = ExecutionTime;
                if (!SetProperty(ref _loopIterations, value))
                {
                    return;
                }
                // Updating Predicted Method Time
                LoopPerformanceInfo.MethodPerformanceInfo.PredictedExecutionTime += ExecutionTime - oldExecutionTime;
                LoopPerformanceInfo.MethodPerformanceInfo.HasChanged = true;
            }
        }

        /// <summary>
        /// The Maximum number of iterations to make available via slider
        /// </summary>
        public int MaxLoopIterations
        {
            get
            {
                if (LoopPerformanceInfo.AverageLoopIterations == 0)
                {
                    return DefaultMaxLoopIterations;
                }
                return LoopPerformanceInfo.AverageLoopIterations * 100;
            }
        }

        public TimeSpan ExecutionTime => LoopPerformanceInfo.GetExecutionTime(LoopIterations);
    }
}
