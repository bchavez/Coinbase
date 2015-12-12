using System;

namespace Coinbase.Mvc
{
    [Obsolete("The Coinbase.Mvc DLL has been deprecated. This specialized MVC DLL is no longer needed. Replace CoinbaseCallback model with Notification model (for callbacks). For MVC projects: Parse the Request.InputStream as a string and the pass the JSON string into api.GetNotification() to get the Notification callback object. For Web API: Simply mount the Notification class as an argument model to your callback API endpoint. You must still verify the callback is from Coinbase.", true)]
    public class CoinbaseCallback
    {
        public Order Order { get; set; }
    }

    public class Order
    {
        
    }
}