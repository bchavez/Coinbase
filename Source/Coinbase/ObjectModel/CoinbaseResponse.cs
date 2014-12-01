namespace Coinbase.ObjectModel
{
    public abstract class CoinbaseResponse
    {
        public bool Success { get; set; }

        public string[] Errors { get; set; }
    }
}