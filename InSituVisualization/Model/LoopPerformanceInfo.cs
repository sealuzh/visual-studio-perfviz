using System;
using System.Collections.Generic;
using InSituVisualization.Utils;

namespace InSituVisualization.Model
{
    public class LoopPerformanceInfo : PerformanceInfo
    {

        public LoopPerformanceInfo(MethodPerformanceInfo methodPerformanceInfo, IList<MethodPerformanceInfo> methodInvocationsPerformanceInfos)
        {
            MethodPerformanceInfo = methodPerformanceInfo ?? throw new ArgumentNullException(nameof(methodPerformanceInfo));
            InvocationPerformanceInfos = methodInvocationsPerformanceInfos ?? throw new ArgumentNullException(nameof(methodInvocationsPerformanceInfos));
        }

        public MethodPerformanceInfo MethodPerformanceInfo { get; }
        public IList<MethodPerformanceInfo> InvocationPerformanceInfos { get; }

        /// <summary>
        /// The Time a single Loop takes
        /// </summary>
        public TimeSpan SingleIterationTime
        {
            get { return InvocationPerformanceInfos.Sum(p => p.MethodPerformanceData.MeanExecutionTime); }
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

        public TimeSpan AverageExecutionTime => GetExecutionTime(AverageLoopIterations);

        public TimeSpan GetExecutionTime(int numberOfIterations)
        {
            return SingleIterationTime.Multiply(numberOfIterations);
        }

    }
}
