namespace Coinbase.ObjectModel
{
    public class OrderResponse : CoinbaseResponse
    {
        public Order Order { get; set; }
    }

    //Added this extra class to avoid Errror vs Errors code smell
    //being under the same parent class.
    public class GetOrderResponse
    {
        public string Error { get; set; }
        public Order Order { get; set; }
    }
}