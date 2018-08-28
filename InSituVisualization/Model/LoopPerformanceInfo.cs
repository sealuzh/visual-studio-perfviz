using System;
using System.Collections.Generic;
using InSituVisualization.Predictions;
using InSituVisualization.Utils;

namespace InSituVisualization.Model
{
    public class LoopPerformanceInfo : PerformanceInfo
    {
        public LoopPerformanceInfo(IPredictionEngine predictionEngine, MethodPerformanceInfo methodPerformanceInfo, IList<MethodPerformanceInfo> methodInvocationsPerformanceInfos)
        {
            PredictionEngine = predictionEngine ?? throw new ArgumentNullException(nameof(predictionEngine));
            MethodPerformanceInfo = methodPerformanceInfo ?? throw new ArgumentNullException(nameof(methodPerformanceInfo));
            InvocationPerformanceInfos = methodInvocationsPerformanceInfos ?? throw new ArgumentNullException(nameof(methodInvocationsPerformanceInfos));
        }

        public IPredictionEngine PredictionEngine { get; }
        public MethodPerformanceInfo MethodPerformanceInfo { get; }
        public IList<MethodPerformanceInfo> InvocationPerformanceInfos { get; }


        public int PredictedLoopIterations { get; set; }

        /// <summary>
        /// The Time a single Loop takes
        /// </summary>
        public TimeSpan SingleIterationTime
        {
            get { return InvocationPerformanceInfos.Sum(p => p.ExecutionTime); }
        }

        /// <summary>
        /// The Average number of Interations in the loop
        /// </summary>
        public int AverageLoopIterations
        {
            get
            {
                if (SingleIterationTime == default(TimeSpan))
                {
                    return 0;
                }
                var timeOfMethod = MethodPerformanceInfo.MethodPerformanceData.MeanExecutionTime.Milliseconds;
                return timeOfMethod / SingleIterationTime.Milliseconds;
            }
        }

        protected override TimeSpan AverageExecutionTime => SingleIterationTime.Multiply(AverageLoopIterations);

        public void PredictExecutionTime()
        {
            // if PredictedLoopIterations = 0 -> we use the default of the average
            PredictedExecutionTime = PredictionEngine.PredictLoopTime(this);
            // Updating Prediction of Method
            MethodPerformanceInfo.PredictExecutionTime();
        }
    }
}
