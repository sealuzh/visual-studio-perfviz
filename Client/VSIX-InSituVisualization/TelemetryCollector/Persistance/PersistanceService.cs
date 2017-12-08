using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VSIX_InSituVisualization.TelemetryCollector.Persistance
{
    static class PersistanceService
    {
        private static readonly string BasePath = Path.GetDirectoryName(Path.GetTempPath()) + "\\InSitu";

        public static bool IsAverageTelemetryLock = false;
        public static bool IsConcreteMemberTelemetriesLock = false;
        
        public static IDictionary<string, IDictionary<string, ConcreteTelemetryMember>> FetchSystemCacheData()
        {
            //TODO: Filename has to match the project
            if (File.Exists(BasePath + "\\VSIXStore.json"))
            {
                var input = File.ReadAllText(BasePath + "\\VSIXStore.json");
                var importedConcreteMemberTelemetires = JsonConvert.DeserializeObject<Dictionary<string, IDictionary<string, ConcreteTelemetryMember>>>(input);
                return importedConcreteMemberTelemetires;
            }
            return new Dictionary<string, IDictionary<string, ConcreteTelemetryMember>>(); //first dict: Key Membername, second dict: Key RestSendID
        }

        public static void WriteSystemCacheData(IDictionary<string, IDictionary<string, ConcreteTelemetryMember>> toStoreTelemetryData)
        {
            //TODO: Find better path because this one is deleted upon startup.
            var json = JsonConvert.SerializeObject(toStoreTelemetryData);
            Directory.CreateDirectory(BasePath);
            File.WriteAllText(BasePath + "\\VSIXStore.json", json);
        }

        public static void AwaitConcreteMemberTelemetriesLock()
        {
            while (IsConcreteMemberTelemetriesLock)
            {
                Task.Delay(50);
            }
        }

        public static void AwaitAverageMemberTelemetryLock()
        {
            while (IsAverageTelemetryLock)
            {
                Task.Delay(50);
            }
        }
    }
}
