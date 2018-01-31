using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.Persistance
{
    public class PersistanceService<T> 
    {
        private static readonly string BasePath = Path.GetDirectoryName(Path.GetTempPath()) + "\\InSitu";
        private static string _fileName; //e.g. VSIXStore.json
        //private static ConcreteMethodTelemetry _instanceType = ConcreteMethodTelemetry;
       // public bool IsAverageTelemetryLock = false;
        //public static bool IsConcreteMemberTelemetriesLock = false;

        public PersistanceService(string fileName)
        {
            //_instanceType = instanceType;
            _fileName = fileName;
        }

        public ConcurrentDictionary<string, ConcurrentDictionary<string, T>> FetchSystemCacheData()
        {
            //TODO: Filename has to match the project
            if (File.Exists(BasePath + "\\" + _fileName))
            {
                var input = File.ReadAllText(BasePath + "\\" + _fileName);
                var importedConcreteMemberTelemetries = JsonConvert.DeserializeObject<ConcurrentDictionary<string, ConcurrentDictionary<string, T>>>(input);
                return importedConcreteMemberTelemetries;
            }
            return new ConcurrentDictionary<string, ConcurrentDictionary<string, T>>(); //first dict: Key Membername, second dict: Key RestSendID
        }

        public void WriteSystemCacheData(object toStoreTelemetryData)
        {
            //TODO: Find better path because this one is deleted upon startup.
            var json = JsonConvert.SerializeObject(toStoreTelemetryData);
            Directory.CreateDirectory(BasePath);
            File.WriteAllText(BasePath + "\\" + _fileName, json);
        }

        //public static async Task AwaitConcreteMemberTelemetriesLock()
        //{
        //    while (IsConcreteMemberTelemetriesLock)
        //    {
        //        await Task.Delay(50);
        //    }
        //}

        //public static async Task AwaitAverageMemberTelemetryLock()
        //{
        //    while (IsAverageTelemetryLock)
        //    {
        //        await Task.Delay(50);
        //    }
        //}
    }
}
