using System;
using System.Diagnostics.CodeAnalysis;
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

        protected async Task<Result<T>> GetAsync<T>(string uri)
        {
            var result = await GetAsync(uri).ConfigureAwait(false);
            if (result.IsFail)
                return result.Error;

            var response = result.Value;
            // Пытаемся получить контент до проверки на успешный статус код, потому что все равно хотелось бы видеть ответ в логах
            // Да и не всегда неуспешный ответ означает отсутствия кода (400 например) 
            var content = await GetContentAsync(response).ConfigureAwait(false);

            if (response.IsSuccessStatusCode && content.IsSuccess) 
                return content.Value.TryDeserialize<T>();
            
            // Для вывода в логи
            var contentMessage = content.IsSuccess ? content.Value : content.Error; 
            Log.Error($"Request failed {uri}: {(int) response.StatusCode} {response.StatusCode.ToString()} {contentMessage}");
            return $"{response.StatusCode.ToString()}: {contentMessage}";
        }

        private async Task<Result<HttpResponseMessage>> GetAsync([NotNull] string uri)
        {
            if (uri == null)
                return "NULL URI";
            
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

        private async Task<Result<string>> GetContentAsync(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return Result<string>.Ok(content);
                
            }
            catch (Exception e)
            {
                return Result<string>.Fail($"Couldn't get response body: {e.Message}");
            }
        }
    }
}