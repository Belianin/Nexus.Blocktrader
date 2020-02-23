using System.Threading.Tasks;
using Blocktrader.Utils;

namespace Blocktrader
{
    public interface IExchangeClient
    {
        Task<Result<OrderBook>> GetOrderBookAsync(Ticket ticket);
        
        Task<Result<float>> GetCurrentAveragePriceAsync(Ticket ticket);
    }
}