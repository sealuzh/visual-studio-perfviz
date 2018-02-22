using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InSituVisualization.TelemetryCollector.Persistance
{
    public class FilePersistentStorage : IPersistentStorage
    {
        private readonly FileInfo _file;

        public FilePersistentStorage(string filePath)
        {
            //_instanceType = instanceType;
            _file = new FileInfo(filePath);
        }

        public async Task<ConcurrentDictionary<string, ConcurrentDictionary<string, T>>> GetDataAsync<T>()
        {
            if (!_file.Exists)
            {
                return new ConcurrentDictionary<string, ConcurrentDictionary<string, T>>(); //first dict: Key Membername, second dict: Key RestSendID
            }

            using (var reader = _file.OpenText())
            {
                var fileText = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<ConcurrentDictionary<string, ConcurrentDictionary<string, T>>>(fileText);
            }
        }

        public async Task StoreDataAsync<T>(ConcurrentDictionary<string, ConcurrentDictionary<string, T>> toStoreTelemetryData)
        {
            _file.Directory?.Create(); // If the directory already exists, this method does nothing.
            
            using (var writer = new StreamWriter(_file.OpenWrite()))
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(toStoreTelemetryData));
            }
        }
    }
}
