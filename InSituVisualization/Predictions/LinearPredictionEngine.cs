using System;
using InSituVisualization.Model;
using InSituVisualization.Utils;

namespace InSituVisualization.Predictions
{
    internal class LinearPredictionEngine : IPredictionEngine
    {
        public LinearPredictionEngine(ISystemWorkload systemWorkload)
        {
            SystemWorkload = systemWorkload;
        }

        public ISystemWorkload SystemWorkload { get; }

        public TimeSpan PredictMethodTime(MethodPerformanceInfo methodPerformanceInfo, object[] parameters)
        {
            // TODO RR: multiply by 1 + workolad
            return methodPerformanceInfo.CalleePerformanceInfo.Sum(p => p.ExecutionTime) + 
                methodPerformanceInfo.LoopPerformanceInfo.Sum(p => p.ExecutionTime);
        }

        public TimeSpan PredictLoopTime(LoopPerformanceInfo loopPerformanceInfo)
        {
            // TODO RR: multiply by 1 + workolad
            return loopPerformanceInfo.SingleIterationTime.Multiply(loopPerformanceInfo.PredictedLoopIterations);
        }
    }
}