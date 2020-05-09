using System.Threading.Tasks;
using Nexus.Blocktrader.Domain;
using Nexus.Blocktrader.Utils;

namespace Nexus.Blocktrader.Exchange
{
    public interface IExchangeClient
    {
        Task<Result<OrderBook>> GetOrderBookAsync(Ticker ticker);
        
        Task<Result<float>> GetCurrentAveragePriceAsync(Ticker ticker);
    }
}