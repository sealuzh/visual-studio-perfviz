using System;
using System.Collections.ObjectModel;
using InSituVisualization.Predictions;
using InSituVisualization.Utils;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.Model
{
    //ReSharper disable UnusedMember.Global
    public class MethodPerformanceInfo : PerformanceInfo
    {
        private readonly ObservableCollection<MethodPerformanceInfo> _callerPerformanceInfos = new SetCollection<MethodPerformanceInfo>();
        private readonly ObservableCollection<MethodPerformanceInfo> _calleePerformanceInfos = new SetCollection<MethodPerformanceInfo>();

        public MethodPerformanceInfo(IPredictionEngine predictionEngine, IMethodSymbol methodSymbol, IMethodPerformanceData methodPerformanceData)
        {
            PredictionEngine = predictionEngine ?? throw new ArgumentNullException(nameof(predictionEngine));
            MethodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
            MethodPerformanceData = methodPerformanceData ?? throw new ArgumentNullException(nameof(methodPerformanceData));
            methodPerformanceData.PropertyChanged += (s, e) => OnPropertyChanged(nameof(ExecutionTime));
            _calleePerformanceInfos.CollectionChanged += (s, e) => PredictExecutionTime();
            LoopPerformanceInfos.CollectionChanged += (s, e) => PredictExecutionTime();
        }

        public IPredictionEngine PredictionEngine { get; }
        public IMethodSymbol MethodSymbol { get; }
        public IMethodPerformanceData MethodPerformanceData { get; }

        /// <summary>
        /// Caller and Callee building the Tree
        /// </summary>
        public ReadOnlyObservableCollection<MethodPerformanceInfo> CallerPerformanceInfos => new ReadOnlyObservableCollection<MethodPerformanceInfo>(_callerPerformanceInfos);
        /// <summary>
        /// Caller and Callee building the Tree
        /// </summary>
        public ReadOnlyObservableCollection<MethodPerformanceInfo> CalleePerformanceInfos => new ReadOnlyObservableCollection<MethodPerformanceInfo>(_calleePerformanceInfos);
        /// <summary>
        /// Loops are inside the Method
        /// </summary>
        public ObservableCollection<LoopPerformanceInfo> LoopPerformanceInfos { get; } = new SetCollection<LoopPerformanceInfo>();

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

        public void AddCalleePerformanceInfo(MethodPerformanceInfo calleePerformanceInfo)
        {
            _calleePerformanceInfos.Add(calleePerformanceInfo);
            calleePerformanceInfo._callerPerformanceInfos.Add(this);
        }

        #region Helper Methods

        protected bool IsPropagating { get; set; }

        /// <summary>
        /// Helper Method for updating callers
        /// Propagating Changes up the Tree
        /// </summary>
        private void PropagateToCallers(Action<MethodPerformanceInfo> action)
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
