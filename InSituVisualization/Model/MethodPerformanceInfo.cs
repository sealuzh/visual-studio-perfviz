using System;
using System.Collections.ObjectModel;
using System.Linq;
using InSituVisualization.Utils;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.Model
{
    public class MethodPerformanceInfo : PerformanceInfo
    {
        private int _numberOfCalls;
        private TimeSpan? _meanExecutionTime;
        private int? _memberCount;
        private bool _hasChanged;


        private readonly ObservableCollection<MethodPerformanceInfo> _callerPerformanceInfo = new SetCollection<MethodPerformanceInfo>();
        private readonly ObservableCollection<MethodPerformanceInfo> _calleePerformanceInfo = new SetCollection<MethodPerformanceInfo>();

        private readonly ObservableCollection<LoopPerformanceInfo> _loopPerformanceInfo = new SetCollection<LoopPerformanceInfo>();

        public MethodPerformanceInfo(IMethodSymbol methodSymbol, MethodPerformanceData methodPerformanceData)
        {
            MethodPerformanceData = methodPerformanceData ?? throw new ArgumentNullException(nameof(methodPerformanceData));
            MethodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
        }

        public IMethodSymbol MethodSymbol { get; }

        public MethodPerformanceData MethodPerformanceData { get; }

        public string MethodName => MethodSymbol.MetadataName;

        public string ContainingType => MethodSymbol.ContainingType?.Name;

        public ObservableCollection<MethodPerformanceInfo> CallerPerformanceInfo => _callerPerformanceInfo;
        public ObservableCollection<MethodPerformanceInfo> CalleePerformanceInfo => _calleePerformanceInfo;
        public ObservableCollection<LoopPerformanceInfo> LoopPerformanceInfo => _loopPerformanceInfo;

        public int NumberOfCalls
        {
            get => _numberOfCalls;
            set => SetProperty(ref _numberOfCalls, value);
        }

        public TimeSpan MeanExecutionTime
        {
            get => _meanExecutionTime ?? GetAverageDuration();
            set => SetProperty(ref _meanExecutionTime, value);
        }

        public int MemberCount
        {
            get => _memberCount ?? MethodPerformanceData.Durations.Count;
            set => SetProperty(ref _memberCount, value);
        }

        /// <summary>
        /// Are there Changes in the MethodText, so that the collected data will not apply anymore
        /// </summary>
        public bool HasChanged
        {
            get => _hasChanged;
            set
            {
                SetProperty(ref _hasChanged, value);
                // Propagating Changes up the Tree
                foreach (var caller in CallerPerformanceInfo)
                {
                    caller.HasChanged = true;
                }
            }
        }

        private TimeSpan GetAverageDuration()
        {
            if (MethodPerformanceData.Durations.Count <= 0)
            {
                return TimeSpan.Zero;
            }
            return TimeSpan.FromMilliseconds(MethodPerformanceData.Durations.Select(telemetry => telemetry.Duration).Average());
        }
    }
}
