using System;
using System.Collections.ObjectModel;
using InSituVisualization.Utils;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.Model
{
    public class MethodPerformanceInfo : PerformanceInfo
    {

        private int _numberOfCalls;
        private TimeSpan _meanExecutionTime;
        private int _memberCount;
        private readonly ObservableCollection<MethodPerformanceInfo> _callerPerformanceInfo = new SetCollection<MethodPerformanceInfo>();
        private readonly ObservableCollection<MethodPerformanceInfo> _calleePerformanceInfo = new SetCollection<MethodPerformanceInfo>();

        public MethodPerformanceInfo(IMethodSymbol methodSymbol)
        {
            MethodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
        }

        public IMethodSymbol MethodSymbol { get; }

        public string MethodName => MethodSymbol.MetadataName;

        public string ContainingType => MethodSymbol.ContainingType?.Name;

        public int NumberOfCalls
        {
            get => _numberOfCalls;
            set => SetProperty(ref _numberOfCalls, value);
        }

        public TimeSpan MeanExecutionTime
        {
            get => _meanExecutionTime;
            set => SetProperty(ref _meanExecutionTime, value);
        }

        public int MemberCount
        {
            get => _memberCount;
            set => SetProperty(ref _memberCount, value);
        }

        public ObservableCollection<MethodPerformanceInfo> CallerPerformanceInfo => _callerPerformanceInfo;

        public ObservableCollection<MethodPerformanceInfo> CalleePerformanceInfo => _calleePerformanceInfo;
    }
}
