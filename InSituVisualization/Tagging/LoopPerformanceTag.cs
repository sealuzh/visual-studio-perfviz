using InSituVisualization.Model;

namespace InSituVisualization.Tagging
{
    public class LoopPerformanceTag : PerformanceTag
    {
        public LoopPerformanceInfo LoopPerformanceInfo { get; }

        public LoopPerformanceTag(LoopPerformanceInfo loopPerformanceInfo) : base(loopPerformanceInfo.MethodPerformanceInfo)
        {
            LoopPerformanceInfo = loopPerformanceInfo;
        }
    }
}
