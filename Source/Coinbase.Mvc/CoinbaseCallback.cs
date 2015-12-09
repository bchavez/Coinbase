using System;
using Coinbase.ObjectModel;

namespace Coinbase.Mvc
{
    [Obsolete("The Coinbase.Mvc DLL has been deprecated. This specialized MVC DLL is no longer needed. Replace CoinbaseCallback model with Notification model (for callbacks) in the main Coinbase library here: https://www.nuget.org/packages/Coinbase/", true)]
    public class CoinbaseCallback
    {
        public Order Order { get; set; }
    }

    public class Order
    {
        
    }
}