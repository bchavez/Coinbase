using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coinbase.Models
{

   public class Entity : Json
   {
      [JsonProperty("id")]
      public string Id { get; set; }

      [JsonProperty("resource")]
      public string Resource { get; set; }

      [JsonProperty("resource_path")]
      public string ResourcePath { get; set; }
   }

   public partial class User : Entity
   {
      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("username")]
      public string Username { get; set; }

      [JsonProperty("profile_location")]
      public string ProfileLocation { get; set; }

      [JsonProperty("profile_bio")]
      public string ProfileBio { get; set; }

      [JsonProperty("profile_url")]
      public string ProfileUrl { get; set; }

      [JsonProperty("avatar_url")]
      public string AvatarUrl { get; set; }
    }

   public partial class AccountCurrency : Json
   {
      [JsonProperty("code")]
      public string Code { get; set; }
      [JsonProperty("name")]
      public string Name { get; set; }
      [JsonProperty("color")]
      public string Color { get; set; }
      [JsonProperty("sort_index")]
      public string SortIndex { get; set; }
      [JsonProperty("exponent")]
      public int Exponent { get; set; }
      [JsonProperty("type")]
      public string Type { get; set; }
      [JsonProperty("address_regex")]
      public string AddressRegex { get; set; }
      [JsonProperty("asset_id")]
      public string AssetId { get; set; }
   }

   public partial class Account : Entity
   {
      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("primary")]
      public bool Primary { get; set; }

      [JsonProperty("type")]
      public string Type { get; set; }

      [JsonProperty("currency")]
      public AccountCurrency Currency { get; set; }

      [JsonProperty("balance")]
      public Money Balance { get; set; }

      [JsonProperty("created_at")]
      public DateTimeOffset CreatedAt { get; set; }

      [JsonProperty("updated_at")]
      public DateTimeOffset UpdatedAt { get; set; }
   }

   public partial class AddressEntity : Entity
   {
      [JsonProperty("address")]
      public string Address { get; set; }

      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("created_at")]
      public DateTimeOffset CreatedAt { get; set; }

      [JsonProperty("updated_at")]
      public DateTimeOffset UpdatedAt { get; set; }

      [JsonProperty("network")]
      public string Network { get; set; }
   }

    public partial class To : Entity
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

   public partial class From : Entity
   {
      [JsonProperty("address")]
      public string Address { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }
   }

   public partial class Transaction : Entity
   {
      [JsonProperty("type")]
      public string Type { get; set; }

      [JsonProperty("status")]
      public string Status { get; set; }

      [JsonProperty("amount")]
      public Money Amount { get; set; }

      [JsonProperty("native_amount")]
      public Money NativeAmount { get; set; }

      [JsonProperty("description")]
      public object Description { get; set; }

      [JsonProperty("created_at")]
      public DateTimeOffset CreatedAt { get; set; }

      [JsonProperty("updated_at")]
      public DateTimeOffset UpdatedAt { get; set; }

      [JsonProperty("network")]
      public Network Network { get; set; }

      [JsonProperty("to")]
      public To To { get; set; }

      [JsonProperty("from")]
      public From From { get; set; }

      [JsonProperty("instant_exchange")]
      public bool InstantExchange { get; set; }

      [JsonProperty("details")]
      public IDictionary<string, JToken> Details { get; set; }

      [JsonProperty("application")]
      public IDictionary<string, JToken> Application { get; set; }
   }

   public partial class Transaction
   {
      [JsonProperty("buy")]
      public Entity Buy { get; set; }
   }



   public partial class Buy : Entity
   {
      [JsonProperty("status")]
      public string Status { get; set; }

      [JsonProperty("payment_method")]
      public Entity PaymentMethod { get; set; }

      [JsonProperty("transaction")]
      public Entity Transaction { get; set; }

      [JsonProperty("amount")]
      public Money Amount { get; set; }

      [JsonProperty("total")]
      public Money Total { get; set; }

      [JsonProperty("subtotal")]
      public Money Subtotal { get; set; }

      [JsonProperty("created_at")]
      public DateTimeOffset CreatedAt { get; set; }

      [JsonProperty("updated_at")]
      public DateTimeOffset UpdatedAt { get; set; }

      [JsonProperty("committed")]
      public bool Committed { get; set; }

      [JsonProperty("instant")]
      public bool Instant { get; set; }

      [JsonProperty("fee")]
      public Money Fee { get; set; }

      [JsonProperty("payout_at")]
      public DateTimeOffset PayoutAt { get; set; }
   }



   public partial class PaymentMethod : Entity
   {
      [JsonProperty("type")]
      public string Type { get; set; }

      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }

      [JsonProperty("primary_buy")]
      public bool PrimaryBuy { get; set; }

      [JsonProperty("primary_sell")]
      public bool PrimarySell { get; set; }

      [JsonProperty("allow_buy")]
      public bool AllowBuy { get; set; }

      [JsonProperty("allow_sell")]
      public bool AllowSell { get; set; }

      [JsonProperty("allow_deposit")]
      public bool AllowDeposit { get; set; }

      [JsonProperty("allow_withdraw")]
      public bool AllowWithdraw { get; set; }

      [JsonProperty("instant_buy")]
      public bool InstantBuy { get; set; }

      [JsonProperty("instant_sell")]
      public bool InstantSell { get; set; }

      [JsonProperty("created_at")]
      public DateTimeOffset CreatedAt { get; set; }

      [JsonProperty("updated_at")]
      public DateTimeOffset UpdatedAt { get; set; }

      [JsonProperty("limits")]
      public IDictionary<string, List<Limit>> Limits { get; set; }
   }


   public partial class Limit
   {
      [JsonProperty("period_in_days")]
      public long PeriodInDays { get; set; }

      [JsonProperty("total")]
      public Money Total { get; set; }

      [JsonProperty("remaining")]
      public Money Remaining { get; set; }
   }




   public partial class Sell : Entity
   {
      [JsonProperty("status")]
      public string Status { get; set; }

      [JsonProperty("payment_method")]
      public Entity PaymentMethod { get; set; }

      [JsonProperty("transaction")]
      public Entity Transaction { get; set; }

      [JsonProperty("amount")]
      public Money Amount { get; set; }

      [JsonProperty("total")]
      public Money Total { get; set; }

      [JsonProperty("subtotal")]
      public Money Subtotal { get; set; }

      [JsonProperty("created_at")]
      public DateTimeOffset CreatedAt { get; set; }

      [JsonProperty("updated_at")]
      public DateTimeOffset UpdatedAt { get; set; }

      [JsonProperty("committed")]
      public bool Committed { get; set; }

      [JsonProperty("instant")]
      public bool Instant { get; set; }

      [JsonProperty("fee")]
      public Money Fee { get; set; }

      [JsonProperty("payout_at")]
      public DateTimeOffset PayoutAt { get; set; }
   }


   public partial class Deposit : Entity
   {
      [JsonProperty("status")]
      public string Status { get; set; }

      [JsonProperty("payment_method")]
      public Entity PaymentMethod { get; set; }

      [JsonProperty("transaction")]
      public Entity Transaction { get; set; }

      [JsonProperty("amount")]
      public Money Amount { get; set; }

      [JsonProperty("subtotal")]
      public Money Subtotal { get; set; }

      [JsonProperty("created_at")]
      public DateTimeOffset CreatedAt { get; set; }

      [JsonProperty("updated_at")]
      public DateTimeOffset UpdatedAt { get; set; }

      [JsonProperty("committed")]
      public bool Committed { get; set; }

      [JsonProperty("fee")]
      public Money Fee { get; set; }

      [JsonProperty("payout_at")]
      public DateTimeOffset PayoutAt { get; set; }
   }



   public partial class Withdrawal : Entity
   {
      [JsonProperty("status")]
      public string Status { get; set; }

      [JsonProperty("payment_method")]
      public Entity PaymentMethod { get; set; }

      [JsonProperty("transaction")]
      public Entity Transaction { get; set; }

      [JsonProperty("amount")]
      public Money Amount { get; set; }

      [JsonProperty("subtotal")]
      public Money Subtotal { get; set; }

      [JsonProperty("created_at")]
      public DateTimeOffset CreatedAt { get; set; }

      [JsonProperty("updated_at")]
      public DateTimeOffset UpdatedAt { get; set; }

      [JsonProperty("committed")]
      public bool Committed { get; set; }

      [JsonProperty("fee")]
      public Money Fee { get; set; }

      [JsonProperty("payout_at")]
      public DateTimeOffset PayoutAt { get; set; }
   }


   public partial class Currency : Json
   {
      [JsonProperty("id")]
      public string Id { get; set; }

      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("min_size")]
      public string MinSize { get; set; }
   }

   public partial class ExchangeRates : Json
   {
      [JsonProperty("currency")]
      public string Currency { get; set; }

      [JsonProperty("rates")]
      public Dictionary<string, decimal> Rates { get; set; }
   }




   public partial class Network : Json
   {
      [JsonProperty("status")]
      public string Status { get; set; }

      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("hash")]
      public string Hash { get; set; }

      [JsonProperty("transaction_fee")]
      public NetworkMoney TransactionFee { get; set; }

      [JsonProperty("transaction_amount")]
      public NetworkMoney TransactionAmount { get; set; }

      [JsonProperty("confirmations")]
      public long Confirmations { get; set; }
   }

   public partial class NetworkMoney : Json
   {
      [JsonProperty("amount")]
      public decimal Amount { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }
   }



   public partial class Money : Json
   {
      [JsonProperty("amount")]
      public decimal Amount { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }

      [JsonProperty("base")]
      public string Base { get; set; }
   }


   public partial class Time : Json
   {
      [JsonProperty("iso")]
      public DateTimeOffset Iso { get; set; }

      [JsonProperty("epoch")]
      public long Epoch { get; set; }
   }



   public partial class Auth : Json
   {
      [JsonProperty("method")]
      public string Method { get; set; }

      [JsonProperty("scopes")]
      public string[] Scopes { get; set; }

      [JsonProperty("oauth_meta")]
      public IDictionary<string, JToken> OauthMeta { get; set; }
   }


   [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
   public partial class UserUpdate
   {
      [JsonProperty("name")]
      public string Name { get; set; }
      [JsonProperty("time_zone")]
      public string TimeZone { get; set; }
      [JsonProperty("native_currency")]
      public string NativeCurrency { get; set; }
   }


   [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
   public partial class UpdateAccount
   {
      [JsonProperty("name")]
      public string Name { get; set; }
   }

   [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
   public partial class CreateAddress
   {
      [JsonProperty("name")]
      public string Name { get; set; }
   }

   [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
   public partial class CreateTransaction
   {
      [JsonProperty("type")]
      public string Type { get; set; } = "send";

      [JsonProperty("to")]
      public string To { get; set; }

      [JsonProperty("amount")]
      public decimal Amount { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }
      [JsonProperty("description")]
      public string Description { get; set; }
      [JsonProperty("skip_notifications")]
      public bool SkipNotifications { get; set; }
      [JsonProperty("fee")]
      public string Fee { get; set; }
      [JsonProperty("idem")]
      public string Idem { get; set; }
      [JsonProperty("to_financial_institution")]
      public bool ToFinancialInstitution { get; set; }
      [JsonProperty("financial_institution_website")]
      public string FinancialInstitutionWebsite { get; set; }

   }

   [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
   public partial class CreateTransfer
   {
      [JsonProperty("type")]
      public string Type { get; set; } = "transfer";

      [JsonProperty("to")]
      public string To { get; set; }

      [JsonProperty("amount")]
      public decimal Amount { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }
      [JsonProperty("description")]
      public string Description { get; set; }
   }

   [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
   public partial class RequestMoney
   {
      [JsonProperty("type")]
      public string Type { get; set; } = "request";

      [JsonProperty("to")]
      public string To { get; set; }

      [JsonProperty("amount")]
      public decimal Amount { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }

      [JsonProperty("description")]
      public string Description { get; set; }
   }


   [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
   public partial class PlaceBuy
   {
      [JsonProperty("amount")]
      public decimal Amount { get; set; }

      [JsonProperty("total")]
      public string Total { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }

      [JsonProperty("payment_method")]
      public string PaymentMethod { get; set; }

      [JsonProperty("agree_btc_amount_varies")]
      public bool AgreeBtcAmountVaries { get; set; }

      [JsonProperty("commit")]
      public bool Commit { get; set; }

      [JsonProperty("quote")]
      public bool Quote { get; set; }
   }

   [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
   public partial class PlaceSell
   {
      [JsonProperty("amount")]
      public decimal Amount { get; set; }

      [JsonProperty("total")]
      public string Total { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }

      [JsonProperty("payment_method")]
      public string PaymentMethod { get; set; }

      [JsonProperty("agree_btc_amount_varies")]
      public bool AgreeBtcAmountVaries { get; set; }

      [JsonProperty("commit")]
      public bool Commit { get; set; }

      [JsonProperty("quote")]
      public bool Quote { get; set; }
   }


   [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
   public partial class DepositFunds
   {
      [JsonProperty("amount")]
      public decimal Amount { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }

      [JsonProperty("payment_method")]
      public string PaymentMethod { get; set; }

      [JsonProperty("commit")]
      public bool Commit { get; set; }
   }

   [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
   public partial class WithdrawalFunds
   {
      [JsonProperty("amount")]
      public decimal Amount { get; set; }

      [JsonProperty("currency")]
      public string Currency { get; set; }

      [JsonProperty("payment_method")]
      public string PaymentMethod { get; set; }

      [JsonProperty("commit")]
      public bool Commit { get; set; }
   }

   public partial class Notification : Entity
   {
      [JsonProperty("type")]
      public string Type { get; set; }

      [JsonProperty("data")]
      public JObject Data { get; set; }

      [JsonProperty("user")]
      public Entity User { get; set; }

      [JsonProperty("account")]
      public Entity Account { get; set; }

      [JsonProperty("delivery_attempts")]
      public int DeliveryAttempts { get; set; }

      [JsonProperty("delivery_response")]
      public JObject DeliveryResponse { get; set; }

      [JsonProperty("created_at")]
      public DateTimeOffset CreatedAt { get; set; }

      [JsonProperty("updated_at")]
      public DateTimeOffset UpdatedAt { get; set; }

      [JsonProperty("additional_data")]
      public JObject AdditionalData { get; set; }
   }


}