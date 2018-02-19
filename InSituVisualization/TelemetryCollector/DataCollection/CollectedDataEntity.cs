//#define DEBUG

using System;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.DataCollection
{
    //TODO JO: Replace concreteMethodTelemetry with this.
    public class CollectedDataEntity
    {
        public Dependency Dependency;
        public Client Client;
        public string Id;
        public DateTime Timestamp;

        private ConcreteMethodTelemetry _mappedConcreteMethodTelemetry;
        private ConcreteMethodException _mappedConcreteMethodException;

        public CollectedDataEntity(dynamic inputTelemetryData)
        {
            Timestamp = Convert.ToDateTime(inputTelemetryData.timestamp);
            Id = (string) inputTelemetryData.id;

            Dependency.Target = (string) inputTelemetryData.dependency.target;
            Dependency.Data = (string)inputTelemetryData.dependency.data;
            Dependency.Success = (bool)inputTelemetryData.dependency.success;
            Dependency.Duration = TimeSpan.FromMilliseconds((double)inputTelemetryData.dependency.duration).Milliseconds;
            Dependency.PerformanceBucket = (string)inputTelemetryData.dependency.performanceBucket;
            Dependency.ResultCode = (int)inputTelemetryData.dependency.resultCode;
            Dependency.Type = (string)inputTelemetryData.dependency.type;
            Dependency.Name = (string)inputTelemetryData.dependency.name;
            Client.Model = (string) inputTelemetryData.client.model;
            Client.Os = (string)inputTelemetryData.client.os;
            Client.Type = (string)inputTelemetryData.client.type;
            Client.Browser = (string)inputTelemetryData.client.browser;
            Client.Ip = (string)inputTelemetryData.client.ip;
            Client.City = (string)inputTelemetryData.client.city;
            Client.StateOrProvince = (string)inputTelemetryData.client.stateOrProvince;
            Client.CountryOrRegion = (string)inputTelemetryData.client.countryOrRegion;

        }

        public ConcreteMethodTelemetry GetConcreteMethodTelemetry()
        {
            return _mappedConcreteMethodTelemetry ?? (_mappedConcreteMethodTelemetry =
                       new ConcreteMethodTelemetry(Dependency.Name, Id, Timestamp, Dependency.Type, Dependency.Duration,
                           Client.City));
        }

        public ConcreteMethodException GetConcreteMethodException()
        {
            return _mappedConcreteMethodException ?? (_mappedConcreteMethodException =
                       new ConcreteMethodException(Dependency.Name, Id, Timestamp, Dependency.Type, Dependency.Data));
        }
    }

    public struct Dependency
    {
        //Method Name of method that called
        public string Target;
        //Errormessage, other data.
        public string Data;
        //Whether the call was successfull
        public bool Success;
        //Duration of the method execution
        public int Duration; //used
        //Unknown
        public string PerformanceBucket;
        //Returned result code of the function (should always be 0)
        public int ResultCode;
        //Exception / Telemetry
        public string Type; //used
        //Methodname
        public string Name; //used

    }

    public struct Client
    {
        //Unknown
        public string Model;
        //OS running on calling system
        public string Os;
        //PC / Mac
        public string Type;
        //Name of browser
        public string Browser;
        //IP Adress
        public string Ip;
        //City Name
        public string City;
        //Region city is in
        public string StateOrProvince;
        //Country of state
        public string CountryOrRegion;
    }
}
