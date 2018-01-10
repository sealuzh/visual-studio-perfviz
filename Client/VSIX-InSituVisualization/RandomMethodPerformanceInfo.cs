using System;
using Microsoft.CodeAnalysis;

namespace VSIX_InSituVisualization
{
    internal class RandomMethodPerformanceInfo : MethodPerformanceInfo
    {
        private static readonly Random Random = new Random();

        public RandomMethodPerformanceInfo(IMethodSymbol methodSymbol) : base(methodSymbol)
        {
            NumberOfCalls = Random.Next();
            MostFrequentCallerName = "Any";
            RecursionDepth = Random.Next();
            MemberTime = Random.NextDouble();
            TotalExecutionTime = TimeSpan.FromMinutes(Random.Next());
            MeanExecutionTime = TimeSpan.FromMilliseconds(Random.Next());
        }
    }
}
