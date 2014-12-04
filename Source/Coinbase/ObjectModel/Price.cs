using Newtonsoft.Json;

namespace Coinbase.ObjectModel
{
    public class Price
    {
        public decimal Cents { get; set; }

        [JsonProperty("currency_iso")]
        public Currency Currency { get; set; }
    }
}