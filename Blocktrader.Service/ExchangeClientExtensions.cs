using System;
using System.Threading.Tasks;
using Blocktrader.Domain;
using Blocktrader.Exchange;

namespace Blocktrader.Service
{
    public static class ExchangeClientExtensions
    {
        public static async Task<TicketInfo> GetTickerInfoAsync(this IExchangeClient client, Ticket ticket)
        {
            try
            {
                var orderBook = await client.GetOrderBookAsync(ticket).ConfigureAwait(false);
                var averagePrice = await client.GetCurrentAveragePriceAsync(ticket).ConfigureAwait(false);

                return new TicketInfo(
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