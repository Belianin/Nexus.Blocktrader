using System;
using System.Threading.Tasks;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Exchange;

namespace Nexus.Blocktrader.Service
{
    public static class ExchangeClientExtensions
    {
        public static async Task<TickerInfo> GetTickerInfoAsync(this IExchangeClient client, Ticker ticker)
        {
            try
            {
                var orderBook = await client.GetOrderBookAsync(ticker).ConfigureAwait(false);
                var averagePrice = await client.GetCurrentAveragePriceAsync(ticker).ConfigureAwait(false);

                return new TickerInfo(
                    averagePrice.IsSuccess ? averagePrice.Value : 0f,
                    orderBook.IsSuccess ? orderBook.Value : new OrderBook(null, null),
                    DateTime.Now); // НЕ ИСПОЛЬЗУТСЯ ПРИ ЗАПИСИ, ПРОСТО НУЖНО ДЛЯ ЧТЕНИЯ
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}