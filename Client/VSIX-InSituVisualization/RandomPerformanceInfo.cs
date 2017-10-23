using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VSIX_InSituVisualization
{
    internal class RandomPerformanceInfo : PerformanceInfo
    {
        private static readonly Random Random = new Random();

        private static readonly IList<PerformanceInfo> StaticRandomPerformanceInfos = new List<PerformanceInfo>
        {
            new RandomPerformanceInfo("MethodName1", 0),
            new RandomPerformanceInfo("MethodName2", 0),
            new RandomPerformanceInfo("MethodName3", 0),
            new RandomPerformanceInfo("MethodName4", 0),
            new RandomPerformanceInfo("MethodName5", 0),
        };

        public RandomPerformanceInfo(string identifierName) : base(identifierName)
        {
            NumberOfCalls = Random.Next();
            MostFrequentCallerName = "Any";
            RecursionDepth = Random.Next();
            MemberTime = Random.NextDouble();
            TotalExecutionTime = TimeSpan.FromMinutes(Random.Next());
            MeanExecutionTime = TimeSpan.FromMilliseconds(Random.Next());

            CallerPerformanceInfo = GetRandomObservableCollection(Random.Next(0, 5));
            CalleePerformanceInfo = GetRandomObservableCollection(Random.Next(0, 5));
        }

        private RandomPerformanceInfo(string identifierName, int numberOfCallerCalleeItems) : base(identifierName)
        {
            NumberOfCalls = Random.Next();
            MostFrequentCallerName = "Any";
            RecursionDepth = Random.Next();
            MemberTime = Random.NextDouble();
            TotalExecutionTime = TimeSpan.FromMinutes(Random.Next());
            MeanExecutionTime = TimeSpan.FromMilliseconds(Random.Next());

            CallerPerformanceInfo = GetRandomObservableCollection(numberOfCallerCalleeItems);
            CalleePerformanceInfo = GetRandomObservableCollection(numberOfCallerCalleeItems);
        }

        public override ObservableCollection<PerformanceInfo> CallerPerformanceInfo { get; }

        public override ObservableCollection<PerformanceInfo> CalleePerformanceInfo { get; }

        private static ObservableCollection<PerformanceInfo> GetRandomObservableCollection(int numberOfItems)
        {
            var observableCollection = new ObservableCollection<PerformanceInfo>();
            for (var i = 0; i < numberOfItems; i++)
            {
                observableCollection.Add(StaticRandomPerformanceInfos[i]);
            }
            return observableCollection;
        }
    }
}
