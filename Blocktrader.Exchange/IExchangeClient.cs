using System.Threading.Tasks;
using Blocktrader.Domain;
using Blocktrader.Utils;

namespace Blocktrader.Exchange
{
    public interface IExchangeClient
    {
        Task<Result<OrderBook>> GetOrderBookAsync(Ticket ticket);
        
        Task<Result<float>> GetCurrentAveragePriceAsync(Ticket ticket);
    }
}