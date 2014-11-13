using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coinbase.ObjectModel
{
    public class Order
    {
        public string Id { get; set; }
        [JsonProperty("created_at")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? CreatedAt { get; set; }

        public Status Status { get; set; }

        [JsonProperty("total_btc")]
        public Price TotalBtc{get; set;}

        [JsonProperty("total_native")]
        public Price TotalNative { get; set; }

        public string Custom { get; set; }

        [JsonProperty( "receive_address" )]
        public string ReceiveAddress { get; set; }

        public ButtonDesc Button { get; set; }

        public Transaction Transaction { get; set; }

        public Customer Customer { get; set; }
    }
}