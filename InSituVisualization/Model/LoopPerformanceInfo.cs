﻿using System;
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

        public TimeSpan SumOfMethodInvocations
        {
            get { return InvocationPerformanceInfos.Sum(p => p.MeanExecutionTime); }
        }

        public int AverageLoopIterations => SumOfMethodInvocations.Milliseconds == 0 ? 0 : MethodPerformanceInfo.MeanExecutionTime.Milliseconds / SumOfMethodInvocations.Milliseconds;
    }
}
