using System;
using System.Globalization;
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

   public partial class CoinbaseApi : FlurlClient, ICoinbaseClient
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

      public CoinbaseApi(OAuthConfig config): this(config as Config){}

      public CoinbaseApi(ApiKeyConfig config) : this(config as Config){}

      public CoinbaseApi() : this(null as Config){}

      /// <summary>
      /// The main class for making Coinbase API calls.
      /// </summary>
      protected CoinbaseApi(Config config)
      {
         this.config = config ?? new Config();
         this.config.EnsureValid();

         this.ConfigureClient();
      }

      private void ApiKeyAuth(ClientFlurlHttpSettings client, ApiKeyConfig keyConfig)
      {
         async Task SetHeaders(HttpCall http)
         {
            var body = http.RequestBody;
            var method = http.Request.Method.Method.ToUpperInvariant();
            var url = http.Request.RequestUri.PathAndQuery;

            string timestamp;
            if (keyConfig.UseTimeApi)
            {
               var timeResult = await this.Data.GetCurrentTimeAsync().ConfigureAwait(false);
               timestamp = timeResult.Data.Epoch.ToString();
            }
            else
            {
               timestamp = ApiKeyAuthenticator.GetCurrentUnixTimestampSeconds().ToString(CultureInfo.CurrentCulture);
            }

            var signature = ApiKeyAuthenticator.GenerateSignature(timestamp, method, url, body, keyConfig.ApiSecret).ToLower();

            http.FlurlRequest
               .WithHeader(HeaderNames.AccessKey, keyConfig.ApiKey)
               .WithHeader(HeaderNames.AccessSign, signature)
               .WithHeader(HeaderNames.AccessTimestamp, timestamp);
         }

         client.BeforeCallAsync = SetHeaders;
      }

      internal static readonly string UserAgent =
         $"{AssemblyVersionInformation.AssemblyProduct}/{AssemblyVersionInformation.AssemblyVersion} ({AssemblyVersionInformation.AssemblyTitle}; {AssemblyVersionInformation.AssemblyDescription})";


      protected internal virtual void ConfigureClient()
      {
         this.WithHeader(HeaderNames.Version, ApiVersionDate)
            .WithHeader("User-Agent", UserAgent)
            .AllowAnyHttpStatus(); //Issue 33

         if (this.config is OAuthConfig oauth)
         {
            this.WithOAuthBearerToken(oauth.OAuthToken);
         }
         if (this.config is ApiKeyConfig key)
         {
            this.Configure(settings => ApiKeyAuth(settings, key));
         }
      }  
   }
}