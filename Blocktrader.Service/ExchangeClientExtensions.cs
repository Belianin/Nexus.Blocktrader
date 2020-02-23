using System.Threading.Tasks;
using Blocktrader.Domain;
using Blocktrader.Exchange;

namespace Blocktrader.Service
{
    public static class ExchangeClientExtensions
    {
        public static async Task<TickerInfo> GetTickerInfoAsync(this IExchangeClient client, Ticket ticket)
        {
            var orderBook = await client.GetOrderBookAsync(ticket).ConfigureAwait(false);
            var averagePrice = await client.GetCurrentAveragePriceAsync(ticket).ConfigureAwait(false);

            return new TickerInfo
            {
                OrderBook = orderBook.IsSuccess ? orderBook.Value : null,
                AveragePrice = averagePrice.IsSuccess ? averagePrice.Value : 0f
            };
        }
    }
}