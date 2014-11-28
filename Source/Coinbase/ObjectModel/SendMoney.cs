using Coinbase.Converters;
using Newtonsoft.Json;

namespace Coinbase.ObjectModel
{
    public class Payment
    {
        /// <summary>
        /// An email address or a bitcoin address.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// A string amount that will be converted to BTC, such as ‘1’ or ‘1.234567’. Also must be >= ‘0.01’ or it will shown an error. If you want to use a different currency you can set amount_string and amount_currency_iso instead. This will automatically convert the amount to BTC and that converted amount will show in the transaction.
        /// </summary>
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? Amount { get; set; }

        /// <summary>
        /// A string amount that can be in any currency. If you use this with amount_currency_iso you should leave Amount null.
        /// </summary>
        [JsonProperty("amount_string")]
        public decimal? AmountString { get; set; }
        
        [JsonProperty("amount_currency_iso")]
        public Currency AmountCurrencyIso { get; set; }

        /// <summary>
        /// Optional notes field. Included in the email that the recipient receives.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Optional parameter signaling that if your account does not currently have enough funds to cover the amount, first purchase the difference with an instant buy, then send the bitcoin.
        /// </summary>
        [JsonProperty("instant_buy")]
        public bool? InstantBuy { get; set; }
    }

    public class SendMoneyRequest
    {
        public Payment transaction { get; set; }
    }
}