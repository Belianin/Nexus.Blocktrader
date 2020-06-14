using System.Text.Json.Serialization;
using Nexus.Blocktrader.Models;
using Nexus.Core;

namespace Nexus.Blocktrader.Api.Models.Responses
{
    public class TradeResponse
    {
        [JsonPropertyName("amount")]
        public float Amount { get; set; }
        [JsonPropertyName("price")]
        public float Price { get; set; }
        [JsonPropertyName("isSale")]
        public bool IsSale { get; set; }
        [JsonPropertyName("time")]
        public long Time { get; set; }

        public static TradeResponse FromTrade(Trade trade)
        {
            return new TradeResponse
            {
                Amount = trade.Amount,
                Price = trade.Price,
                IsSale = trade.IsSale,
                Time = trade.Time.ToUnixTimeMilliseconds()
            };
        }
    }
}