namespace Coinbase.ObjectModel
{
    public enum Status
    {
        Canceled,
        Completed = 0x01,
        New,
        Mispaid,
        Pending
    }
}