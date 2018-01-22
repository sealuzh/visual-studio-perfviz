using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.Persistance
{
    static class PersistanceService
    {
        private static readonly string BasePath = Path.GetDirectoryName(Path.GetTempPath()) + "\\InSitu";

        public static bool IsAverageTelemetryLock = false;
        public static bool IsConcreteMemberTelemetriesLock = false;

        public static IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> FetchSystemCacheData()
        {
            //TODO: Filename has to match the project
            if (File.Exists(BasePath + "\\VSIXStore.json"))
            {
                var input = File.ReadAllText(BasePath + "\\VSIXStore.json");
                var importedConcreteMemberTelemetires = JsonConvert.DeserializeObject<Dictionary<string, IDictionary<string, ConcreteMethodTelemetry>>>(input);
                return importedConcreteMemberTelemetires;
            }
            return new Dictionary<string, IDictionary<string, ConcreteMethodTelemetry>>(); //first dict: Key Membername, second dict: Key RestSendID
        }

        public static void WriteSystemCacheData(IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> toStoreTelemetryData)
        {
            //TODO: Find better path because this one is deleted upon startup.
            var json = JsonConvert.SerializeObject(toStoreTelemetryData);
            Directory.CreateDirectory(BasePath);
            File.WriteAllText(BasePath + "\\VSIXStore.json", json);
        }

        //public static async Task AwaitConcreteMemberTelemetriesLock()
        //{
        //    while (IsConcreteMemberTelemetriesLock)
        //    {
        //        await Task.Delay(50);
        //    }
        //}

        public static async Task AwaitAverageMemberTelemetryLock()
        {
            while (IsAverageTelemetryLock)
            {
                await Task.Delay(50);
            }
        }
    }
}
