using System;
using Coinbase.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coinbase.ObjectModel
{
    public class MoneyAmount
    {
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }

    public class UserAddress
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class TransactionDesc
    {
        public string Id { get; set; }
        
        [JsonProperty("created_at")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? CreatedAt { get; set; }

        public string Hsh { get; set; }

        public string Notes { get; set; }
        public string Idem { get; set; }
        public MoneyAmount Amount { get; set; }
        public bool Request { get; set; }
        public Status Status { get; set; }
        public UserAddress Sender { get; set; }
        public UserAddress Recipient { get; set; }

        [JsonProperty("recipient_address")]
        public string RecipientAddress { get; set; }
    }

    public class Fees
    {
        public Price Coinbase { get; set; }
        public Price Bank { get; set; }
    }

    public class TransferDesc
    {
        public string Id { get; set; }
        [JsonProperty("created_at")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? CreatedAt { get; set; }
        public Fees Fees { get; set; }

        [JsonProperty("payout_date")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? PayoutDate { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        public string _type { get; set; }
        public string Code { get; set; }
        public string type { get; set; }
        public Status Status { get; set; }

        public MoneyAmount Btc { get; set; }
        public MoneyAmount Subtotal { get; set; }
        public MoneyAmount Total { get; set; }
        public string Description { get; set; }
    }

    public class SendMoneyResponse : CoinbaseResponse
    {
        public TransactionDesc Transaction { get; set; }
        public TransferDesc Transfer { get; set; }
    }
}
