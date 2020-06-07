using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Nexus.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Blocktrader.Worker
{
    public abstract class StatefulWorker<T> : BackgroundService where T : class
    {
        protected readonly T State;

        protected StatefulWorker()
        {
            State = GetState();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RunAsync(stoppingToken).ConfigureAwait(false);
        }
        
        protected abstract Task RunAsync(CancellationToken token);

        private T GetState()
        {
            var filename = GetType().Name + ".json";
            var path = $"{Directory.GetCurrentDirectory()}\\State";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fullPath = path + "\\" + filename;
            if (!File.Exists(fullPath))
                throw new IOException($"Couldn't find state \"{fullPath}\"");

            var text = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<T>(text);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            SaveState(State);
        }

        private void SaveState(T state)
        {
            var path = $"{Directory.GetCurrentDirectory()}\\State\\{GetType().Name}.json";

            var text = JsonConvert.SerializeObject(state, Formatting.Indented);
            File.WriteAllText(path, text);
        }
    }
}