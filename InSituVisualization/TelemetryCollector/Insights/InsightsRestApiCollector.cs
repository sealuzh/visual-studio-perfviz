﻿using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.Model;

namespace InSituVisualization.TelemetryCollector.Insights
{
    // ReSharper disable once ClassNeverInstantiated.Global, Justification: IoC
    internal class InsightsRestApiCollector : ITelemetryCollector
    {
        private readonly InsightsRestApiClient _client;
        private readonly InsightsRestApiDataMapper _mapper;

        public InsightsRestApiCollector(InsightsRestApiClient client, InsightsRestApiDataMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<IList<RecordedMethodTelemetry>> GetTelemetryAsync()
        {
            var response = await _client.GetTelemetryAsync().ConfigureAwait(false);
            return _mapper.GetMethodTelemetry(response);
        }
    }
}
