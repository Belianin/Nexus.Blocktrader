using System;

namespace Blocktrader
{
    public interface IExchange
    {
        ExchangeInfo GetInfo();

        Ticket Ticket { get; set; }

        void ForceUpdate();
        
        event EventHandler OnUpdate;
    }
}