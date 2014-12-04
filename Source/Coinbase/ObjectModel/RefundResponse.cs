using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coinbase.ObjectModel
{
    public class Mispayment
    {
        public string Id { get; set; }
        [JsonProperty("created_at")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("total_btc")]
        public Price TotalBtc { get; set; }

        [JsonProperty("total_native")]
        public Price TotalNative { get; set; }

        [JsonProperty("refund_transaction")]
        public Transaction RefundTransaction { get; set; }
    }

    public class RefundOrder : Order
    {
        [JsonProperty("refund_address")]
        public string RefundAddress { get; set; }

        public object Event { get; set; }

        [JsonProperty("total_payout")]
        public Price TotalPayout { get; set; }

        [JsonProperty("refund_transaction")]
        public Transaction RefundTransaction { get; set; }

        [JsonProperty("mispaid_btc")]
        public Price MispaidBtc { get; set; }

        [JsonProperty("mispaid_native")]
        public Price MispaidNative { get; set; }

        public Mispayment[] Mispayments { get; set; }

        public string[] Errors { get; set; }
    }

    /// <summary>
    ///If the order has status completed and the refund processed successfully, the order data will contain the refund transaction details in RefundTransaction.
    /// </summary>
    public class RefundResponse : CoinbaseResponse
    {
        [JsonProperty("order")]
        public RefundOrder Order { get; set; }
    }
}
