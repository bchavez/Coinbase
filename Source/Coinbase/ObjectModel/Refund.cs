using Newtonsoft.Json;

namespace Coinbase.ObjectModel
{
    public class RefundRequest
    {
        [JsonProperty("order")]
        public RefundOptions RefundOptions { get; set; }
    }

    /// <summary>
    /// Authenticated resource which refunds an order or a mispayment to an order. Returns a snapshot of the order data, updated with refund transaction details.
    /// This endpoint will only refund the full amount of the order or mispayment, specified as either the original BTC amount or native currency amount (such as USD). To issue partial refunds, you can use the regular api/v1/transactions/send_money endpoint.
    /// If the order has status completed and the refund processed successfully, the order data will contain the refund transaction details as order['refund_transaction'].
    /// If one of the order’s mispayments was refunded, the order data will contain the refund transaction as part of that mispayment’s data. E.g. order['mispayments'][0]['refund_transaction'].
    /// By default, refunds will be issued to the refund_address that is set on the order or the mispayment. This field is automatically present when the original incoming transaction was from a Coinbase user, or via the payment protocol. In these cases, we are able to provide a refund address automatically. If the refund_address is not present, you can specify an address to send the refund to with the external_refund_address parameters.
    /// If the refund does not process, order['errors'] will be present, specifying any problems.
    /// </summary>
    public class RefundOptions
    {
        /// <summary>
        /// The currency to issue the refund in. If BTC, the original bitcoin amount will be sent back. If USD (or another currency code if the order had a different native price), the amount of bitcoin sent back will be equivalent to the original USD value (or other native value) at the current exchange rate.
        /// </summary>
        [JsonProperty("refund_iso_code")]
        public string RefundIsoCurrency { get; set; }

        /// <summary>
        /// This field is required if the order or mispayment does not already have a value for refund_address. Must be a valid bitcoin address. If this field is specified but the order or mispayment already has a refund_address that was automatically added by Coinbase, the already-present refund_address will take precendence over the external_refund_address specified.
        /// </summary>
        [JsonProperty("external_refund_address")]
        public string ExternalRefundAddress { get; set; }
        
        /// <summary>
        /// The ID of a mispayment to be refunded. If left blank, the original order transaction will be refunded, if the order status is completed.
        /// </summary>
        [JsonProperty("mispayment_id")]
        public string MisspaymentId { get; set; }

        /// <summary>
        /// If true, will make an instant purchase for any amount of bitcoin attempting to be sent that is not already available in the account balance.
        /// </summary>
        [JsonProperty("instant_buy")]
        public bool InstantBuy { get; set; }
    }
}
