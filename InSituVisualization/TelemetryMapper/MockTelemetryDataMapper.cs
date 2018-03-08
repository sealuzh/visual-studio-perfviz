using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using InSituVisualization.Annotations;
using InSituVisualization.Model;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.TelemetryMapper
{
    /// <summary>
    /// Returns Mock Method Data
    /// </summary>
    internal class MockTelemetryDataMapper : ITelemetryDataMapper
    {
        private class MockPerformanceData : IMethodPerformanceData
        {
            private static readonly Random Random = new Random();

            public ObservableCollection<RecordedExceptionMethodTelemetry> Exceptions { get; } = new ObservableCollection<RecordedExceptionMethodTelemetry>();
            public ObservableCollection<RecordedExecutionTimeMethodTelemetry> ExecutionTimes { get; } = new ObservableCollection<RecordedExecutionTimeMethodTelemetry>();
            public IList<RecordedExecutionTimeMethodTelemetry> FilteredExecutionTimes => ExecutionTimes;
            public TimeSpan MeanExecutionTime { get; } = TimeSpan.FromMilliseconds(Random.Next(100));
            public TimeSpan TotalExecutionTime { get; } = TimeSpan.FromMilliseconds(Random.Next(100));
            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private class MockMethodPerformanceInfo : MethodPerformanceInfo
        {
            public MockMethodPerformanceInfo(IMethodSymbol methodSymbol) : base( methodSymbol, new MockPerformanceData())
            {
            }
        }

        private readonly Dictionary<string, MethodPerformanceInfo> _telemetryDatas = new Dictionary<string, MethodPerformanceInfo>();

        public Task<MethodPerformanceInfo> GetMethodPerformanceInfoAsync(IMethodSymbol methodSymbol)
        {
            // DocumentationCommentId is used in Symbol Editor, since methodSymbols aren't equal accross compilations
            // see https://github.com/dotnet/roslyn/issues/3058
            var documentationCommentId = methodSymbol.GetDocumentationCommentId();
            if (_telemetryDatas.TryGetValue(documentationCommentId, out var performanceInfo))
            {
                return Task.FromResult(performanceInfo);
            }
            var newPerformanceInfo = new MockMethodPerformanceInfo(methodSymbol);
            _telemetryDatas.Add(documentationCommentId, newPerformanceInfo);
            return Task.FromResult((MethodPerformanceInfo) newPerformanceInfo);
        }
    }
}
