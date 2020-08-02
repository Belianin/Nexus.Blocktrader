using System.Text.Json.Serialization;

namespace Nexus.Blocktrader.Api.Models.Responses
{
    public class TradesMonthStatisticsResponse
    {
        [JsonPropertyName("bidsAmount")]
        public float BidsAmount { get; set; }
        
        [JsonPropertyName("asksAmount")]
        public float AsksAmount { get; set; }
    }
}