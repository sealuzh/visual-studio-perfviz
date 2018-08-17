using System;
using InSituVisualization.Model;

namespace InSituVisualization.Predictions
{
    public interface IPredictionEngine
    {
        TimeSpan PredictMethodTime(MethodPerformanceInfo methodPerformanceInfo, object[] parameters);
        TimeSpan PredictLoopTime(LoopPerformanceInfo loopPerformanceInfo);
    }
}
