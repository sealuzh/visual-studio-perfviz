using System;
using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis;
using VSIX_InSituVisualization.Utils;

namespace VSIX_InSituVisualization.Model
{
    public class MethodPerformanceInfo : ModelBase
    {
        private readonly IMethodSymbol _methodSymbol;
        private int _numberOfCalls;
        private string _mostFrequentCallerName;
        private int _recursionDepth;
        private double _memberTime;
        private TimeSpan _totalExecutionTime;
        private TimeSpan _meanExecutionTime;
        private int _memberCount;

        public MethodPerformanceInfo(IMethodSymbol methodSymbol)
        {
            _methodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
            MethodName = methodSymbol.MetadataName;
            ContainingType = methodSymbol.ContainingType?.Name;
        }

        public string MethodName { get; }

        public string ContainingType { get; }

        public int NumberOfCalls
        {
            get => _numberOfCalls;
            set => SetProperty(ref _numberOfCalls, value);
        }

        public string MostFrequentCallerName
        {
            get => _mostFrequentCallerName;
            set => SetProperty(ref _mostFrequentCallerName, value);
        }

        public int RecursionDepth
        {
            get => _recursionDepth;
            set => SetProperty(ref _recursionDepth, value);
        }

        /// <summary>
        /// The percentage of total runtime the method has been active
        /// depicted as a value and color-coded in the background of the main rectangle on a 
        /// scale from light green light green (low) to dark red(high).
        /// </summary>
        public double MemberTime
        {
            get => _memberTime;
            set => SetProperty(ref _memberTime, value);
        }

        public TimeSpan TotalExecutionTime
        {
            get => _totalExecutionTime;
            set => SetProperty(ref _totalExecutionTime, value);
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

        public ObservableCollection<MethodPerformanceInfo> CallerPerformanceInfo { get; } = new SetCollection<MethodPerformanceInfo>();

        public ObservableCollection<MethodPerformanceInfo> CalleePerformanceInfo { get; } = new SetCollection<MethodPerformanceInfo>();

    }
}
