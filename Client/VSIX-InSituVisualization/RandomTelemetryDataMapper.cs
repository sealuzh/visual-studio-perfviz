namespace VSIX_InSituVisualization
{
    internal class RandomTelemetryDataMapper : ITelemetryDataMapper
    {
        public PerformanceInfo GetPerformanceInfo(string memberIdentification)
        {
            return new RandomPerformanceInfo(memberIdentification);
        }
    }
}
