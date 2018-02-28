using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.Persistance
{

    public class FilePersistentStorage : IPersistentStorage
    {
        private readonly FileInfo _file;

        public FilePersistentStorage()
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Path.GetTempPath()) + "\\InSitu", "VSIX_Telemetries.json");
            _file = new FileInfo(filePath);
        }

        public async Task<ConcurrentDictionary<string, T>> GetDataAsync<T>()
        {
            if (!_file.Exists)
            {
                return new ConcurrentDictionary<string, T>();
            }

            using (var reader = _file.OpenText())
            {
                var fileText = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<ConcurrentDictionary<string, T>>(fileText);
            }
        }

        public async Task StoreDataAsync<T>(ConcurrentDictionary<string, T> toStoreTelemetryData)
        {
            _file.Directory?.Create();
            using (var writer = new StreamWriter(_file.OpenWrite()))
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(toStoreTelemetryData));
            }
        }
    }
}
