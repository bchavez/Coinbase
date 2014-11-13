using Newtonsoft.Json;

namespace Coinbase.ObjectModel
{
    public class Customer
    {
        public string Email { get; set; }

        [JsonProperty("shipping_address")]
        public string[] ShippingAddress { get; set; }
    }
}