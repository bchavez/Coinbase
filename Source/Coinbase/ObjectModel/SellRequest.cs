using Coinbase.Converters;
using Newtonsoft.Json;

namespace Coinbase.ObjectModel
{
    public class SellRequest
    {
        public SellRequest()
        {
        }

        /// <summary>
        /// Specify which account is used to sell from. The default is your primary account
        /// </summary>
        [JsonProperty("account_id")]
        public string AccountId { get; set; }
        
        /// <summary>
        /// Required: The quantity of bitcoin you would like to buy.
        /// </summary>
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal Qty { get; set; }

        /// <summary>
        /// Currency of qty, must be either BTC or the currency of the payment method
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// Defaults to true. If set to false, this buy will not be immediately completed. Use the POST /transfers/:id/commit call to complete it
        /// </summary>
        public bool? Commit { get; set; }

        /// <summary>
        /// Whether or not you would still like to buy if you have to wait for your money to arrive to lock in a price.
        /// </summary>
        [JsonProperty("agree_btc_amount_varies")]
        public bool? AgreeBtcAmoutVaries { get; set; }

        /// <summary>
        /// The ID of the payment method that should be used for the sell. Payment methods can be listed using the /payment_methods API call.
        /// </summary>
        [JsonProperty("payment_method_id")]
        public string PaymentMethodId { get; set; }
    }

    public class SellResponse : CoinbaseResponse
    {
        public TransferDesc Transfer { get; set; }
    }
}