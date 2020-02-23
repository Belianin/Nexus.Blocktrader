using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blocktrader.Utils;
using Blocktrader.Utils.Logging;
using Newtonsoft.Json;

namespace Blocktrader.Exchange
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
            Log.Debug($"Got response from {uri}: {(int) response.StatusCode} {response.StatusCode.ToString()}");

            return response;
        }

        protected async Task<Result<T>> GetAsync<T>(string uri)
        {
            var response = await GetAsync(uri).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            if (response.IsSuccessStatusCode)
                return content.TryDeserialize<T>();

            return $"{response.StatusCode.ToString()}: {content}";
        }
    }
}