namespace Nexus.Blocktrader.Exchanges.Binance.Models
{
    public class TradeResponse
    {
        public int Id { get; set; }
        public float Price { get; set; }
        public string Qty { get; set; }
        public string QuoteQty { get; set; }
        public long Time { get; set; }
        public bool IsBuyerMaker { get; set; }
        public bool IsBestMatch { get; set; }
    }
}