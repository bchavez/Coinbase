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

        /// <summary>
        /// Use this field to associate this transaction with an order as a refund. 
        /// This transaction’s ID will appear in the order’s manual_refund_tx_ids field.
        /// </summary>
        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// An optional token to ensure idempotence. 
        /// If a previous transaction with the same idem parameter already exists for this sender, that previous transaction will be returned and a new one will not be created. 
        /// Max length 100 characters.
        /// </summary>
        [JsonProperty("idem")]
        public string Idem { get; set; }

        /// <summary>
        /// Specify which account is debited. The default is your primary account.
        /// </summary>
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        /// <summary>
        /// Optional transaction fee if you would like to pay it. 
        /// Coinbase pays transaction fees on payments greater than or equal to 0.01 BTC. 
        /// But for smaller amounts you may want to add your own amount. 
        /// Fees can be added as a string, such as ‘0.0005’.
        /// </summary>
        [JsonProperty("user_fee")]
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal? UserFee { get; set; }

        /// <summary>
        /// Optional id of the user to get a referral credit in the case that this transaction makes the user eligible. 
        /// The referring user is eligible for a credit if the address in the ‘to’ field is an email address for which there is currently no registered account and the recipient proceeds to buy or sell at least 1 BTC.
        /// </summary>
        [JsonProperty("referrer_id")]
        public string ReferrerId { get; set; }
    }

    public class SendMoneyRequest
    {
        public Payment transaction { get; set; }
    }
}