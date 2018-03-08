﻿using System;
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
            _predictedExecutionTime = MethodPerformanceData.MeanExecutionTime;
            methodPerformanceData.PropertyChanged += (s, e) => OnMethodPerformanceDataChanged();
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
                SetProperty(ref _predictedExecutionTime, value);

                // Propagating Changes up the Tree
                foreach (var caller in _callerPerformanceInfo)
                {
                    caller.PredictedExecutionTime = caller.PredictedExecutionTime - oldValue + value;
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
                SetProperty(ref _hasChanged, value);
                // Propagating Changes up the Tree
                foreach (var caller in _callerPerformanceInfo)
                {
                    caller.HasChanged = value;
                }
            }
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
