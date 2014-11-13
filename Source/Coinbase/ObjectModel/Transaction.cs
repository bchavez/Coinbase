namespace Coinbase.ObjectModel
{
    public class Transaction
    {
        public string Id { get; set; }
        public string Hash { get; set; }
        public int Confirmations { get; set; }
    }
}