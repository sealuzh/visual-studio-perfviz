using System;
using System.Linq;
using InSituVisualization.Model;
using InSituVisualization.Utils;

namespace InSituVisualization.Predictions
{
    /// <summary>
    /// The most simple Prediction Engine
    /// </summary>
    internal class LinearPredictionEngine : IPredictionEngine
    {
        public LinearPredictionEngine(ISystemWorkload systemWorkload)
        {
            SystemWorkload = systemWorkload;
        }

        public ISystemWorkload SystemWorkload { get; }

        /// <summary>
        /// Predicts Method Execution Time
        /// sums up callee times for methods
        /// </summary>
        public TimeSpan PredictMethodTime(MethodPerformanceInfo methodPerformanceInfo, object[] parameters)
        {
            // TODO RR: multiply by 1 + workolad
            var executionTimeWithoutLoops = methodPerformanceInfo.CalleePerformanceInfos.Sum(p => p.ExecutionTime);
            if (!methodPerformanceInfo.LoopPerformanceInfos.Any())
            {
                return executionTimeWithoutLoops;
            }
            var singleIterationTime = methodPerformanceInfo.LoopPerformanceInfos.Sum(p => p.SingleIterationTime);
            var predictedLoopTime = methodPerformanceInfo.LoopPerformanceInfos.Sum(p => p.ExecutionTime);
            return executionTimeWithoutLoops - singleIterationTime + predictedLoopTime;
        }

        public TimeSpan PredictLoopTime(LoopPerformanceInfo loopPerformanceInfo)
        {
            // TODO RR: multiply by 1 + workolad
            return loopPerformanceInfo.SingleIterationTime.Multiply(loopPerformanceInfo.PredictedLoopIterations);
        }
    }
}