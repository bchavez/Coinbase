namespace Coinbase.ObjectModel
{
    public class Amount
    {
        public string amount { get; set; }
        public string currency { get; set; }
    }

    public class UserAddress
    {
        public string id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
    }

    public class SendMoneyTransactionResponse
    {
        public string id { get; set; }
        public string created_at { get; set; }
        public object hsh { get; set; }
        public Amount amount { get; set; }
        public bool request { get; set; }
        public string status { get; set; }
        public UserAddress sender { get; set; }
        public UserAddress recipient { get; set; }
        public string recipient_address { get; set; }
        public string notes { get; set; }
        public string idem { get; set; }
        public string[] Errors { get; set; }
    }

    public class Fees
    {
        public Amount coinbase { get; set; }
        public Amount bank { get; set; }
    }

    public class Transfer
    {
        public string id { get; set; }
        public string created_at { get; set; }
        public Fees fees { get; set; }
        public string payout_date { get; set; }
        public string transaction_id { get; set; }
        public string _type { get; set; }
        public string code { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public Amount btc { get; set; }
        public Amount subtotal { get; set; }
        public Amount total { get; set; }
        public string description { get; set; }
    }

    public class SendMoneyResponse
    {
        public bool success { get; set; }
 
            public SendMoneyTransactionResponse transaction { get; set; }

            public Transfer transfer { get; set; }
 
    }
}
