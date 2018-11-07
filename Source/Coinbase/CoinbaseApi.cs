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

      protected internal Url AccountsEndpoint;
      protected internal Url PaymentMethodsEndpoint;
      protected internal Url CurrenciesEndpoint;
      protected internal Url ExchangeRatesEndpoint;
      protected internal Url PricesEndpoint;
      protected internal Url TimeEndpoint;
      protected internal Url NotificationsEndpoint;

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

         this.AccountsEndpoint = this.config.ApiUrl.AppendPathSegment("accounts");
         this.PaymentMethodsEndpoint = this.config.ApiUrl.AppendPathSegment("payment-methods");
         this.CurrenciesEndpoint = this.config.ApiUrl.AppendPathSegment("currencies");
         this.ExchangeRatesEndpoint = this.config.ApiUrl.AppendPathSegment("exchange-rates");
         this.PricesEndpoint = this.config.ApiUrl.AppendPathSegment("prices");
         this.TimeEndpoint = this.config.ApiUrl.AppendPathSegment("time");
         this.NotificationsEndpoint = this.config.ApiUrl.AppendPathSegment("notifications");
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