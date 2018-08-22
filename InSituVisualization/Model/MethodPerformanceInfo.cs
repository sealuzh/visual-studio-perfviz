using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InSituVisualization.Predictions;
using InSituVisualization.Utils;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.Model
{
    /// <summary>
    /// All Information about a methods performance, including <see cref="IMethodPerformanceData"/> and Predicitons/>
    /// TODO RR: Maybe implement Green/Red Tree like roslyn. Immutable PerformanceInfos would help the performance...
    /// </summary>
    //ReSharper disable UnusedMember.Global
    public class MethodPerformanceInfo : PerformanceInfo
    {
        /// <summary>
        /// Unique Callers
        /// </summary>
        private readonly ObservableCollection<MethodPerformanceInfo> _callerPerformanceInfos = new SetCollection<MethodPerformanceInfo>();
        /// <summary>
        /// Unique Callees
        /// </summary>
        private readonly ObservableCollection<MethodPerformanceInfo> _calleePerformanceInfos = new SetCollection<MethodPerformanceInfo>();
        /// <summary>
        /// All Callees
        /// </summary>
        private ObservableCollection<MethodPerformanceInfo> _allCalleePerformanceInfo = new ObservableCollection<MethodPerformanceInfo>();
        /// <summary>
        /// Loops (loops are always unique, since they are no symbol..)
        /// </summary>
        private ObservableCollection<LoopPerformanceInfo> _loopPerformanceInfos = new ObservableCollection<LoopPerformanceInfo>();

        public MethodPerformanceInfo(IPredictionEngine predictionEngine, IMethodSymbol methodSymbol, IMethodPerformanceData methodPerformanceData)
        {
            PredictionEngine = predictionEngine ?? throw new ArgumentNullException(nameof(predictionEngine));
            MethodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
            MethodPerformanceData = methodPerformanceData ?? throw new ArgumentNullException(nameof(methodPerformanceData));
            methodPerformanceData.PropertyChanged += (s, e) => OnPropertyChanged(nameof(ExecutionTime));
            //_calleePerformanceInfos.CollectionChanged += (s, e) => PredictExecutionTime();
            //LoopPerformanceInfos.CollectionChanged += (s, e) => PredictExecutionTime();
        }

        public IPredictionEngine PredictionEngine { get; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global - Justification: WPF Binding
        public IMethodSymbol MethodSymbol { get; }
        public IMethodPerformanceData MethodPerformanceData { get; }

        /// <summary>
        /// Caller and Callee building the Tree (unique)
        /// </summary>
        public ReadOnlyObservableCollection<MethodPerformanceInfo> CallerPerformanceInfos => new ReadOnlyObservableCollection<MethodPerformanceInfo>(_callerPerformanceInfos);
        /// <summary>
        /// Caller and Callee building the Tree (unique)
        /// </summary>
        public ReadOnlyObservableCollection<MethodPerformanceInfo> CalleePerformanceInfos => new ReadOnlyObservableCollection<MethodPerformanceInfo>(_calleePerformanceInfos);
        /// <summary>
        /// Loops are inside the Method (unique)
        /// </summary>
        public ReadOnlyObservableCollection<LoopPerformanceInfo> LoopPerformanceInfos => new ReadOnlyObservableCollection<LoopPerformanceInfo>(_loopPerformanceInfos);
        /// <summary>
        /// Non unique Callees
        /// </summary>
        public ReadOnlyObservableCollection<MethodPerformanceInfo> AllCalleePerformanceInfo => new ReadOnlyObservableCollection<MethodPerformanceInfo>(_allCalleePerformanceInfo);

        protected override TimeSpan AverageExecutionTime => MethodPerformanceData.MeanExecutionTime;

        /// <summary>
        /// The Prediction is the sum of all callees (if not set differently)
        /// </summary>
        public void PredictExecutionTime()
        {
            // TODO RR: Do not use Set, Use Loop Infos as well
            PredictedExecutionTime = PredictionEngine.PredictMethodTime(this, new object[0]);
            // Propagating Changes up the Tree
            PropagateToCallers(mpi => mpi.PredictExecutionTime());
        }


        public void UpdateCallees(IList<MethodPerformanceInfo> invocationsList)
        {
            _allCalleePerformanceInfo = new ObservableCollection<MethodPerformanceInfo>(invocationsList);
            // ToList needed here to stop Except from being executed in a deferred manner
            var removedPerformanceInfo = CalleePerformanceInfos.Except(invocationsList).ToList();
            foreach (var performanceInfo in removedPerformanceInfo)
            {
                if (_allCalleePerformanceInfo.Contains(performanceInfo))
                {
                    // another call still exists
                    continue;
                }
                _calleePerformanceInfos.Remove(performanceInfo);
                performanceInfo._callerPerformanceInfos.Remove(this);
            }
            var addedPerformanceInfo = invocationsList.Except(CalleePerformanceInfos).ToList();
            foreach (var performanceInfo in addedPerformanceInfo)
            {
                _calleePerformanceInfos.Add(performanceInfo);
                performanceInfo._callerPerformanceInfos.Add(this);
            }
        }

        public void UpdateLoops(IList<LoopPerformanceInfo> loopsList)
        {
            _loopPerformanceInfos = new ObservableCollection<LoopPerformanceInfo>(loopsList);
        }

        #region Propagation

        private bool IsPropagating { get; set; }

        /// <summary>
        /// Helper Method for updating callers
        /// Propagating Changes up the Tree
        /// </summary>
        public void PropagateToCallers(Action<MethodPerformanceInfo> action)
        {
            IsPropagating = true;
            foreach (var caller in _callerPerformanceInfos)
            {
                if (caller.IsPropagating)
                {
                    // do not update if already updating ... (otherwise infinite loop...)
                    continue;
                }
                action(caller);
            }
            IsPropagating = false;
        }

        #endregion
    }
    //ReSharper restore UnusedMember.Global
}
