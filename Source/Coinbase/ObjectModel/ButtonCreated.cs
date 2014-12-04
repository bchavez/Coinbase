namespace Coinbase.ObjectModel
{
    public class ButtonCreated : ButtonRequest
    {
        public string Code { get; set; }
        public new Price Price { get; set; }

        public new Currency Currency
        {
            get
            {
                return this.Price.Currency;
            }
        }
    }
}