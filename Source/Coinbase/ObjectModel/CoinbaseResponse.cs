namespace Coinbase.ObjectModel
{
    public abstract class CoinbaseResponse
    {
        public bool Success { get; set; }

        public string[] Errors { get; set; }

        public string Error { get; set; }

        public bool HasErrors
        {
            get { return (Error != null || Errors != null); }
        }
    }
}