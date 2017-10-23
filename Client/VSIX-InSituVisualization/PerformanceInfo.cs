using System;
using System.Collections.ObjectModel;

namespace VSIX_InSituVisualization
{
    public abstract class PerformanceInfo : ModelBase
    {

        private int _numberOfCalls;
        private string _mostFrequentCallerName;
        private int _recursionDepth;
        private double _memberTime;
        private TimeSpan _totalExecutionTime;
        private TimeSpan _meanExecutionTime;

        protected PerformanceInfo(string identifierName)
        {
            IdentifierName = identifierName;
        }

        public string IdentifierName { get; }

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

        public abstract ObservableCollection<PerformanceInfo> CallerPerformanceInfo { get; }

        public abstract ObservableCollection<PerformanceInfo> CalleePerformanceInfo { get; }

    }
}
