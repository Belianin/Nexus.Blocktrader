using System.Threading.Tasks;
using Nexus.Blocktrader.Models;
using Nexus.Core;

namespace Nexus.Blocktrader.Exchanges
{
    public interface IExchangeClient
    {
        Task<Result<OrderBook>> GetOrderBookAsync(Ticker ticker);
        
        Task<Result<float>> GetCurrentAveragePriceAsync(Ticker ticker);

        Task<Result<Trade[]>> GetLastTradesAsync(Ticker ticker);
    }
}