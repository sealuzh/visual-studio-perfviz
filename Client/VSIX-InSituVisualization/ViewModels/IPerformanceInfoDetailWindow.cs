using VSIX_InSituVisualization.Model;

namespace VSIX_InSituVisualization.ViewModels
{
    interface IPerformanceInfoDetailWindow
    {
        void ShowMethodPerformance(MethodPerformanceInfo methodPerformanceInfo);
        void ShowLoopPerformance(LoopPerformanceInfo loopPerformanceinfo);
    }
}
