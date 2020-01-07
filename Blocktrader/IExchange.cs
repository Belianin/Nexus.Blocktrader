using System;

namespace Blocktrader
{
    public interface IExchange
    {
        ExchangeInfo GetInfo();
        
        event EventHandler OnUpdate;
    }
}