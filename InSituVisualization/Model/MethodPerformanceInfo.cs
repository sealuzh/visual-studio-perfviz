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
        private TimeSpan _predictedExecutionTime;

        public MethodPerformanceInfo(IMethodSymbol methodSymbol, IMethodPerformanceData methodPerformanceData)
        {
            MethodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
            MethodPerformanceData = methodPerformanceData ?? throw new ArgumentNullException(nameof(methodPerformanceData));
            methodPerformanceData.PropertyChanged += (s, e) => OnMethodPerformanceDataChanged();
            _calleePerformanceInfo.CollectionChanged += (s, e) => UpdatePredictedExecutionTime();
        }

        public IMethodSymbol MethodSymbol { get; }

        public IMethodPerformanceData MethodPerformanceData { get; }

        /// <summary>
        /// Caller and Callee building the Tree
        /// </summary>
        public ReadOnlyObservableCollection<MethodPerformanceInfo> CallerPerformanceInfo => new ReadOnlyObservableCollection<MethodPerformanceInfo>(_callerPerformanceInfo);
        /// <summary>
        /// Caller and Callee building the Tree
        /// </summary>
        public ReadOnlyObservableCollection<MethodPerformanceInfo> CalleePerformanceInfo => new ReadOnlyObservableCollection<MethodPerformanceInfo>(_calleePerformanceInfo);

        public ObservableCollection<LoopPerformanceInfo> LoopPerformanceInfo { get; } = new SetCollection<LoopPerformanceInfo>();

        public TimeSpan ExecutionTime => HasChanged ? PredictedExecutionTime : MethodPerformanceData.MeanExecutionTime;


        public TimeSpan PredictedExecutionTime
        {
            get => _predictedExecutionTime;
            set
            {
                var oldValue = _predictedExecutionTime;
                var difference = oldValue + value;
                SetProperty(ref _predictedExecutionTime, value);

                // Propagating Changes up the Tree
                foreach (var caller in _callerPerformanceInfo)
                {
                    if (caller == this)
                    {
                        // do not update self... (otherwise it may result in a stackoverflow)
                        continue;
                    }
                    caller.PredictedExecutionTime = caller.PredictedExecutionTime - difference;
                }

                OnPropertyChanged(nameof(ExecutionTime));
            }
        }

        /// <summary>
        /// Are there Changes in the MethodText, so that the collected data will not apply anymore
        /// </summary>
        public bool HasChanged
        {
            get => _hasChanged;
            set
            {
                if (!SetProperty(ref _hasChanged, value))
                {
                    return;
                }

                if (value)
                {
                    UpdatePredictedExecutionTime();
                }
                // Propagating Changes up the Tree
                foreach (var caller in _callerPerformanceInfo)
                {
                    caller.HasChanged = value;
                }
                OnPropertyChanged(nameof(ExecutionTime));
            }
        }

        /// <summary>
        /// The Prediction is the sum of all callees (if not set differently)
        /// </summary>
        private void UpdatePredictedExecutionTime()
        {
            // TODO RR: Do not use Set, Use Loop Infos as well
            PredictedExecutionTime = CalleePerformanceInfo.Sum(p => p.ExecutionTime);
        }

        public void AddCalleePerformanceInfo(MethodPerformanceInfo calleePerformanceInfo)
        {
            _calleePerformanceInfo.Add(calleePerformanceInfo);
            calleePerformanceInfo._callerPerformanceInfo.Add(this);
        }

        private void OnMethodPerformanceDataChanged()
        {
            OnPropertyChanged(nameof(ExecutionTime));
        }

    }
    //ReSharper restore UnusedMember.Global
}
