using Newtonsoft.Json;

namespace Coinbase.ObjectModel
{
    public class Refund
    {
        [JsonProperty("order")]
        public OrderRefund OrderPart { get; set; }

        public Refund(string externalRefundAddress, Currency refundCurrency)
        {
            OrderPart = new OrderRefund
                {
                    ExternalRefundAddress = externalRefundAddress,
                    RefundIsoCurrency = refundCurrency.ToString()
                };
        }
    }

    public class OrderRefund
    {
        [JsonProperty("refund_iso_code")]
        public string RefundIsoCurrency { get; set; }

        [JsonProperty("external_refund_address")]
        public string ExternalRefundAddress { get; set; }
    }
}
