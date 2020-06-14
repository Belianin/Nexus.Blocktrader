namespace Nexus.Blocktrader.Exchanges.Bitstamp.Models
{
    public class TradeResponse
    {
        public long Date { get; set; }
        public int Tid { get; set; }
        public float Price { get; set; }
        public float Amount { get; set; }
        public int Type { get; set; }
    }
}