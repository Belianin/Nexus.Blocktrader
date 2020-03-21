using System.Threading.Tasks;
using Blocktrader.Domain;
using Blocktrader.Utils;

namespace Blocktrader.Exchange
{
    public interface IExchangeClient
    {
        Task<Result<OrderBook>> GetOrderBookAsync(Ticker ticker);
        
        Task<Result<float>> GetCurrentAveragePriceAsync(Ticker ticker);
    }
}