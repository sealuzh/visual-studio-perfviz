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
        private readonly ObservableCollection<MethodPerformanceInfo> _callerPerformanceInfo = new SetCollection<MethodPerformanceInfo>();
        private readonly ObservableCollection<MethodPerformanceInfo> _calleePerformanceInfo = new SetCollection<MethodPerformanceInfo>();

        public MethodPerformanceInfo(IPredictionEngine predictionEngine, IMethodSymbol methodSymbol, IMethodPerformanceData methodPerformanceData)
        {
            PredictionEngine = predictionEngine ?? throw new ArgumentNullException(nameof(predictionEngine));
            MethodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
            MethodPerformanceData = methodPerformanceData ?? throw new ArgumentNullException(nameof(methodPerformanceData));
            methodPerformanceData.PropertyChanged += (s, e) => OnPropertyChanged(nameof(ExecutionTime));
            _calleePerformanceInfo.CollectionChanged += (s, e) => PredictExecutionTime();
        }

        public IPredictionEngine PredictionEngine { get; }
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

        protected override TimeSpan AverageExecutionTime => MethodPerformanceData.MeanExecutionTime;

        /// <summary>
        /// The Prediction is the sum of all callees (if not set differently)
        /// </summary>
        public void PredictExecutionTime()
        {
            // TODO RR: Do not use Set, Use Loop Infos as well
            PredictedExecutionTime = PredictionEngine.PredictMethodTime(this, new object[0]);
            // Propagating Changes up the Tree
            DoForCallers(mpi => mpi.PredictExecutionTime());
        }

        public void AddCalleePerformanceInfo(MethodPerformanceInfo calleePerformanceInfo)
        {
            _calleePerformanceInfo.Add(calleePerformanceInfo);
            calleePerformanceInfo._callerPerformanceInfo.Add(this);
        }

        #region Helper Methods

        /// <summary>
        /// Helper Method for updating callers
        /// Propagating Changes up the Tree
        /// </summary>
        private void DoForCallers(Action<MethodPerformanceInfo> action)
        {
            foreach (var caller in _callerPerformanceInfo)
            {
                if (caller == this)
                {
                    // do not update self... (otherwise it may result in a stackoverflow)
                    continue;
                }
                action(caller);
            }
        }

        #endregion
    }
    //ReSharper restore UnusedMember.Global
}
