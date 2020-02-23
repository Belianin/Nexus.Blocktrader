using System.Net.Http;
using System.Threading.Tasks;
using Blocktrader.Utils;
using Blocktrader.Utils.Logging;
using Newtonsoft.Json;

namespace Blocktrader
{
    public abstract class BaseClient
    {
        protected readonly ILog Log;
        private readonly HttpClient httpClient;

        protected BaseClient(ILog log)
        {
            Log = log;
            httpClient = new HttpClient();
        }

        protected async Task<HttpResponseMessage> GetAsync(string uri)
        {
            Log.Debug($"Sending request {uri}");
            var response = await httpClient.GetAsync(uri).ConfigureAwait(false);
            Log.Debug($"Got response from {uri}: {response.StatusCode.ToString()}");

            return response;
        }

        protected async Task<Result<T>> GetAsync<T>(string uri)
        {
            var response = await GetAsync(uri).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(response.Content.ToString());
            }

            return $"{response.StatusCode.ToString()}: {response.Content.ToString()}";
        }
    }
}