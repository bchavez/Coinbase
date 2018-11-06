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
   public partial class CoinbaseApi : IDisposable
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

         this.client = this.GetNewClient();

         this.AccountsEndpoint = this.config.ApiUrl.AppendPathSegment("accounts");
         this.PaymentMethodsEndpoint = this.config.ApiUrl.AppendPathSegment("payment-methods");
         this.CurrenciesEndpoint = this.config.ApiUrl.AppendPathSegment("currencies");
         this.ExchangeRatesEndpoint = this.config.ApiUrl.AppendPathSegment("exchange-rates");
         this.PricesEndpoint = this.config.ApiUrl.AppendPathSegment("prices");
         this.TimeEndpoint = this.config.ApiUrl.AppendPathSegment("time");
         this.NotificationsEndpoint = this.config.ApiUrl.AppendPathSegment("notifications");
      }

      private void ApiKeyAuth(ClientFlurlHttpSettings client, ApiKeyConfig config)
      {
         async Task SetHeaders(HttpCall http)
         {
            var body = http.RequestBody;
            var method = http.Request.Method.Method.ToUpperInvariant();
            var url = http.Request.RequestUri.PathAndQuery;

            string timestamp;
            if (config.UseTimeApi)
            {
               var timeResult = await this.Data.GetCurrentTimeAsync().ConfigureAwait(false);
               timestamp = timeResult.Data.Epoch.ToString();
            }
            else
            {
               timestamp = ApiKeyAuthenticator.GetCurrentUnixTimestampSeconds().ToString(CultureInfo.CurrentCulture);
            }

            var signature = ApiKeyAuthenticator.GenerateSignature(timestamp, method, url, body, config.ApiSecret).ToLower();

            http.FlurlRequest
               .WithHeader(HeaderNames.AccessKey, config.ApiKey)
               .WithHeader(HeaderNames.AccessSign, signature)
               .WithHeader(HeaderNames.AccessTimestamp, timestamp);
         }

         client.BeforeCallAsync = SetHeaders;
      }

      internal static readonly string UserAgent =
         $"{AssemblyVersionInformation.AssemblyProduct}/{AssemblyVersionInformation.AssemblyVersion} ({AssemblyVersionInformation.AssemblyTitle}; {AssemblyVersionInformation.AssemblyDescription})";

      private IFlurlClient client;

      protected internal virtual IFlurlClient CreateClient()
      {
         return new FlurlClient()
            .WithHeader(HeaderNames.Version, ApiVersionDate)
            .WithHeader("User-Agent", UserAgent);
      }

      /// <summary>
      /// Get the underlying configured client to make raw HTTP calls.
      /// </summary>
      public IFlurlClient GetCurrentClient()
      {
         return this.client;
      }

      /// <summary>
      /// Get a new configured client to make raw HTTP calls.
      /// </summary>
      public IFlurlClient GetNewClient()
      {
         var c = this.CreateClient();

         if( this.config is OAuthConfig oauth )
         {
            return c.WithOAuthBearerToken(oauth.OAuthToken);
         }
         if( this.config is ApiKeyConfig key )
         {
            return c.Configure(settings => ApiKeyAuth(settings, key));
         }

         return c;
      }
      

      public void Dispose()
      {
         client?.Dispose();
      }
   }
}