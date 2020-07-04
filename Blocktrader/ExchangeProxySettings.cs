namespace Nexus.Blocktrader
{
    public class ExchangeProxySettings
    {
        public string Host { get; set; }
        public int Port { get; set; } = 80;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; }
    }
}