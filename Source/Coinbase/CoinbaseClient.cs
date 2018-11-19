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

      protected internal readonly Config config;

      public const string Endpoint = "https://api.coinbase.com/v2/";

      protected internal Url AccountsEndpoint => this.config.ApiUrl.AppendPathSegment("accounts");
      protected internal Url PaymentMethodsEndpoint => this.config.ApiUrl.AppendPathSegment("payment-methods");
      protected internal Url CurrenciesEndpoint => this.config.ApiUrl.AppendPathSegment("currencies");
      protected internal Url ExchangeRatesEndpoint => this.config.ApiUrl.AppendPathSegment("exchange-rates");
      protected internal Url PricesEndpoint => this.config.ApiUrl.AppendPathSegment("prices");
      protected internal Url TimeEndpoint => this.config.ApiUrl.AppendPathSegment("time");
      protected internal Url NotificationsEndpoint => this.config.ApiUrl.AppendPathSegment("notifications");

      public CoinbaseClient(OAuthConfig config): this(config as Config){}

      public CoinbaseClient(ApiKeyConfig config) : this(config as Config){}

      public CoinbaseClient() : this(null as Config){}

      /// <summary>
      /// The main class for making Coinbase API calls.
      /// </summary>
      protected CoinbaseClient(Config config)
      {
         this.config = config ?? new Config();
         this.config.EnsureValid();
         
         this.ConfigureClient();
      }


      internal static readonly string UserAgent =
         $"{AssemblyVersionInformation.AssemblyProduct}/{AssemblyVersionInformation.AssemblyVersion} ({AssemblyVersionInformation.AssemblyTitle}; {AssemblyVersionInformation.AssemblyDescription})";

      protected internal virtual void ConfigureClient()
      {
         this.WithHeader(HeaderNames.Version, ApiVersionDate)
              .WithHeader("User-Agent", UserAgent);

         this.config.Configure(this);
      }
   }
}
