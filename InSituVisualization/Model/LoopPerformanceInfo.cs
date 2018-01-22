using System;
using System.Collections.Generic;
using InSituVisualization.Utils;

namespace InSituVisualization.Model
{
    public class LoopPerformanceInfo : PerformanceInfo
    {

        public LoopPerformanceInfo(MethodPerformanceInfo methodPerformanceInfo, IList<MethodPerformanceInfo> methodInvocationsPerformanceInfos)
        {
            ParentMethodPerformanceInfo = methodPerformanceInfo ?? throw new ArgumentNullException(nameof(methodPerformanceInfo));
            ChildMethodInvocationsPerformanceInfos = methodInvocationsPerformanceInfos ?? throw new ArgumentNullException(nameof(methodInvocationsPerformanceInfos));
        }

        public MethodPerformanceInfo ParentMethodPerformanceInfo { get; }
        public IList<MethodPerformanceInfo> ChildMethodInvocationsPerformanceInfos { get; }

        public TimeSpan SumOfMethodInvocations
        {
            get { return ChildMethodInvocationsPerformanceInfos.Sum(p => p.MeanExecutionTime); }
        }

        public int AverageLoopIterations => SumOfMethodInvocations.Milliseconds == 0 ? 0 : ParentMethodPerformanceInfo.MeanExecutionTime.Milliseconds / SumOfMethodInvocations.Milliseconds;
    }
}
