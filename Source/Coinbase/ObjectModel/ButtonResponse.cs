namespace Coinbase.ObjectModel
{
    public class ButtonResponse : CoinbaseResponse
    {
        public ButtonCreated Button { get; set; }

		protected internal string CheckoutPageUrl { get; set; }

        public string GetCheckoutUrl()
        {
            var url = this.CheckoutPageUrl
                .Replace("{code}", Button.Code);

            return url;
        }
    }
}