using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace Coinbase
{
   public interface ICoinbaseClient : IFlurlClient
   {
      IAccountsEndpoint Accounts { get; }
      IAddressesEndpoint Addresses { get; }
      IBuysEndpoint Buys { get; }
      IDataEndpoint Data { get; }
      IDepositsEndpoint Deposits { get; }
      INotificationsEndpoint Notifications { get; }
      IPaymentMethodsEndpoint PaymentMethods { get; }
      ISellsEndpoint Sells { get; }
      ITransactionsEndpoint Transactions { get; }
      IUsersEndpoint Users { get; }
      IWithdrawalsEndpoint Withdrawals { get; }
   }

   public partial class CoinbaseClient : FlurlClient, ICoinbaseClient
   {
      public const string ApiVersionDate = "2017-08-07";

      public Config Config { get; }

      public const string Endpoint = "https://api.coinbase.com/v2/";

      protected internal Url AccountsEndpoint => this.Config.ApiUrl.AppendPathSegment("accounts");
      protected internal Url PaymentMethodsEndpoint => this.Config.ApiUrl.AppendPathSegment("payment-methods");
      protected internal Url CurrenciesEndpoint => this.Config.ApiUrl.AppendPathSegment("currencies");
      protected internal Url ExchangeRatesEndpoint => this.Config.ApiUrl.AppendPathSegment("exchange-rates");
      protected internal Url PricesEndpoint => this.Config.ApiUrl.AppendPathSegment("prices");
      protected internal Url TimeEndpoint => this.Config.ApiUrl.AppendPathSegment("time");
      protected internal Url NotificationsEndpoint => this.Config.ApiUrl.AppendPathSegment("notifications");

      /// <summary>
      /// The main class for making Coinbase API calls.
      /// </summary>
      public CoinbaseClient(Config config = null)
      {
         this.Config = config ?? new Config();
         this.Config.EnsureValid();
         
         this.ConfigureClient();
      }


      internal static readonly string UserAgent =
         $"{AssemblyVersionInformation.AssemblyProduct}/{AssemblyVersionInformation.AssemblyVersion} ({AssemblyVersionInformation.AssemblyTitle}; {AssemblyVersionInformation.AssemblyDescription})";

      protected internal virtual void ConfigureClient()
      {
         this.WithHeader(HeaderNames.Version, ApiVersionDate)
              .WithHeader("User-Agent", UserAgent);

         this.Config.Configure(this);
      }

      public interface ICoinbaseApiClient : ICoinbaseClient
      {
      }

      public class CoinbaseApiClient : CoinbaseClient, ICoinbaseApiClient
      {
         public CoinbaseApiClient(ApiKeyConfig config) : base(config)
         {
         }
      }

      public interface ICoinbaseOAuthClient : ICoinbaseClient
      {
      }

      public class CoinbaseOAuthClient : CoinbaseClient, ICoinbaseOAuthClient
      {
         public CoinbaseOAuthClient(OAuthConfig config) : base(config)
         {
         }
      }
   }
}
