using System;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    public class CollectedDataEntity
    {
        public CollectedDataEntity(dynamic inputTelemetryData)
        {
            Id = (string)inputTelemetryData.id;
            Timestamp = Convert.ToDateTime(inputTelemetryData.timestamp);
            ClientData = new ClientData(inputTelemetryData);
            DependencyData = new DependencyData(inputTelemetryData);
        }

        public string Id { get; }
        public DateTime Timestamp { get; }
        public ClientData ClientData { get; }
        public DependencyData DependencyData { get; }
    }

    public class DependencyData
    {

        public DependencyData(dynamic inputTelemetryData)
        {
            Target = (string)inputTelemetryData.dependency.target;
            Data = (string)inputTelemetryData.dependency.data;
            Success = (bool)inputTelemetryData.dependency.success;
            Duration = TimeSpan.FromMilliseconds((double)inputTelemetryData.dependency.duration).Milliseconds;
            PerformanceBucket = (string)inputTelemetryData.dependency.performanceBucket;
            ResultCode = (int)inputTelemetryData.dependency.resultCode;
            Type = (string)inputTelemetryData.dependency.type;
            Name = (string)inputTelemetryData.dependency.name;
        }

        //Method Name of method that called
        public string Target { get; }
        //Errormessage, other data.
        public string Data { get; }
        //Whether the call was successfull
        public bool Success { get; }
        //Duration of the method execution
        public int Duration { get; }
        //Unknown
        public string PerformanceBucket { get; }
        //Returned result code of the function (should always be 0)
        public int ResultCode { get; }
        //Exception / Telemetry
        public string Type { get; }
        //Methodname
        public string Name { get; }

    }
}
