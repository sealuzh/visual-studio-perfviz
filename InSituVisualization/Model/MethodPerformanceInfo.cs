using System;
using System.Collections.ObjectModel;
using InSituVisualization.Utils;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.Model
{
    //ReSharper disable UnusedMember.Global
    public class MethodPerformanceInfo : PerformanceInfo
    {
        private bool _hasChanged;

        private readonly ObservableCollection<MethodPerformanceInfo> _callerPerformanceInfo = new SetCollection<MethodPerformanceInfo>();
        private readonly ObservableCollection<MethodPerformanceInfo> _calleePerformanceInfo = new SetCollection<MethodPerformanceInfo>();
        private TimeSpan _predictedMeanExecutionTime;

        public MethodPerformanceInfo(IMethodSymbol methodSymbol, MethodPerformanceData methodPerformanceData)
        {
            MethodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
            MethodPerformanceData = methodPerformanceData ?? throw new ArgumentNullException(nameof(methodPerformanceData));
            _predictedMeanExecutionTime = MethodPerformanceData.MeanExecutionTime;
        }

        public IMethodSymbol MethodSymbol { get; }

        public MethodPerformanceData MethodPerformanceData { get; }

        /// <summary>
        /// Caller and Callee building the Tree
        /// </summary>
        public ReadOnlyObservableCollection<MethodPerformanceInfo> CallerPerformanceInfo => new ReadOnlyObservableCollection<MethodPerformanceInfo>(_callerPerformanceInfo);
        /// <summary>
        /// Caller and Callee building the Tree
        /// </summary>
        public ReadOnlyObservableCollection<MethodPerformanceInfo> CalleePerformanceInfo => new ReadOnlyObservableCollection<MethodPerformanceInfo>(_calleePerformanceInfo);

        public ObservableCollection<LoopPerformanceInfo> LoopPerformanceInfo { get; } = new SetCollection<LoopPerformanceInfo>();

        public TimeSpan MeanExecutionTime
        {
            get
            {
                if (!HasChanged)
                {
                    return MethodPerformanceData.MeanExecutionTime;
                }
                return PredictedMeanExecutionTime;
            }
        }


        public TimeSpan PredictedMeanExecutionTime
        {
            get => _predictedMeanExecutionTime;
            set
            {
                var oldValue = _predictedMeanExecutionTime;
                SetProperty(ref _predictedMeanExecutionTime, value);

                // Propagating Changes up the Tree
                foreach (var caller in _callerPerformanceInfo)
                {
                    caller.PredictedMeanExecutionTime = caller.PredictedMeanExecutionTime - oldValue + value;
                }

                OnPropertyChanged(nameof(MeanExecutionTime));
            }
        }


        public void AddCalleePerformanceInfo(MethodPerformanceInfo calleePerformanceInfo)
        {
            _calleePerformanceInfo.Add(calleePerformanceInfo);
            calleePerformanceInfo._callerPerformanceInfo.Add(this);
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
                foreach (var caller in _callerPerformanceInfo)
                {
                    caller.HasChanged = value;
                }
            }
        }

    }
    //ReSharper restore UnusedMember.Global
}
