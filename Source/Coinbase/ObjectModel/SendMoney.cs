namespace Coinbase.ObjectModel
{
    public class SendMoneyTransaction
    {
        public string to { get; set; }

        public string amount { get; set; }

        public string notes { get; set; }
    }

    public class SendMoneyRequest
    {
        public SendMoneyTransaction transaction { get; set; }
    }
}