using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.Model;
using InSituVisualization.TelemetryCollector.DataCollection;

namespace InSituVisualization.TelemetryCollector
{
    // ReSharper disable once ClassNeverInstantiated.Global Justification: IOC
    internal class TelemetryProvider : ITelemetryProvider
    {
        private static readonly TimeSpan ThrottleTimeSpan = TimeSpan.FromMinutes(1);
        private readonly DataCollectionServiceProvider _dataCollectionServiceProvider;

        private DateTime _lastUpdate = default(DateTime);

        public TelemetryProvider(DataCollectionServiceProvider dataCollectionServiceProvider)
        {
            _dataCollectionServiceProvider = dataCollectionServiceProvider;
        }

        private HashSet<string> AcknowledgedTelemetryIds { get; } = new HashSet<string>();
        private Dictionary<string, MethodPerformanceData> TelemetryByDocumentationCommentId { get; } = new Dictionary<string, MethodPerformanceData>();

        public async Task<MethodPerformanceData> GetTelemetryDataAsync(string documentationCommentId)
        {
            await UpdateTelemetryDataAsync();
            return TelemetryByDocumentationCommentId.TryGetValue(documentationCommentId, out var methodTelemetry) ? methodTelemetry : null;
        }

        private async Task UpdateTelemetryDataAsync()
        {
            if (_lastUpdate + ThrottleTimeSpan > DateTime.Now)
            {
                return;
            }
            _lastUpdate = DateTime.Now;

            foreach (var dataCollector in _dataCollectionServiceProvider.GetDataCollectionServices())
            {
                var recordedMethodTelemetries = await dataCollector.GetTelemetryAsync();
                foreach (var methodTelemetry in recordedMethodTelemetries)
                {
                    if (!AcknowledgedTelemetryIds.Add(methodTelemetry.Id) || string.IsNullOrWhiteSpace(methodTelemetry.DocumentationCommentId))
                    {
                        // Telemetry is already known or no documentationCommentId
                        continue;
                    }

                    TelemetryByDocumentationCommentId.TryGetValue(methodTelemetry.DocumentationCommentId, out var performanceData);
                    if (performanceData == null)
                    {
                        performanceData = new MethodPerformanceData();
                        TelemetryByDocumentationCommentId.Add(methodTelemetry.DocumentationCommentId, performanceData);
                    }

                    switch (methodTelemetry)
                    {
                        case RecordedExecutionTimeMethodTelemetry recordedDurationMethodTelemetry:
                            performanceData.ExecutionTimes.Add(recordedDurationMethodTelemetry);
                            break;
                        case RecordedExceptionMethodTelemetry exceptionTelemetry:
                            performanceData.Exceptions.Add(exceptionTelemetry);
                            break;
                    }
                }
            }
        }



        // TODO RR: Filters
        //private readonly FilterController<T> _filterController = new FilterController<T>();

        //public void AddDebugFilters()
        //{
        //    _filterController.AddFilter(
        //        _filterController.GetFilterProperties()[3],
        //        FilterKind.IsGreaterEqualThen, new DateTime(2018, 1, 15, 12, 45, 00));
        //}
        //        public ConcurrentDictionary<string, T> CurrentMethodTelemetries => _filterController.ApplyFilters(AllMethodTelemetries);



    }
}
