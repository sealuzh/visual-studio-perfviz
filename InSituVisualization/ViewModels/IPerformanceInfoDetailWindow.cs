using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    interface IPerformanceInfoDetailWindow
    {
        void ShowMethodPerformance(MethodPerformanceInfo methodPerformanceInfo);
        void ShowLoopPerformance(LoopPerformanceInfo loopPerformanceinfo);
    }
}
