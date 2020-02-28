using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blocktrader.Utils;
using Blocktrader.Utils.Logging;

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

        private async Task<Result<HttpResponseMessage>> GetAsync(string uri)
        {
            Log.Debug($"Sending request {uri}");
            try
            {
                var response = await httpClient.GetAsync(uri).ConfigureAwait(false);
                Log.Debug($"Got response from {uri}: {(int) response.StatusCode} {response.StatusCode.ToString()}");

                return response;
            }
            catch (HttpRequestException e)
            {
                Log.Error($"Failed to get response from {uri}: {e.Message}");
                return e.Message;
            }
        }

        protected async Task<Result<T>> GetAsync<T>(string uri)
        {
            var result = await GetAsync(uri).ConfigureAwait(false);
            if (result.IsFail)
                return result.Error;

            var response = result.Value;
            
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            if (response.IsSuccessStatusCode)
                return content.TryDeserialize<T>();

            Log.Error($"Request failed {uri}: {(int) response.StatusCode} {response.StatusCode.ToString()} {content}");
            return $"{response.StatusCode.ToString()}: {content}";
        }
    }
}