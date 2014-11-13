namespace Coinbase.ObjectModel
{
    public class ButtonResponse : CoinbaseResponse
    {
        public ButtonCreated Button { get; set; }

        public string GetCheckoutUrl()
        {
            var url = CoinbaseApi.CheckoutPageUrl
                .Replace("{code}", Button.Code);

            return url;
        }
    }
}