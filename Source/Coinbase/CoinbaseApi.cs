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
   public class Config
   {
      public string ApiKey { get; set; }
      public string ApiSecret { get; set; }
      public string OAuthToken { get; set; }
      public string ApiUrl { get; set; } = CoinbaseApi.Endpoint;
      public bool UseTimeApi { get; set; } = true;
   }
   public partial class CoinbaseApi : IDisposable
   {
      public const string ApiVersionDate = "2017-08-07";

      protected internal readonly string apiKey;
      protected internal readonly string apiSecret;
      protected internal readonly string oauthToken;
      protected internal readonly bool useTimeApi;

      public const string Endpoint = "https://api.coinbase.com/v2/";

      protected internal string apiUrl;

      protected internal Url AccountsEndpoint;
      protected internal Url PaymentMethodsEndpoint;
      protected internal Url CurrenciesEndpoint;
      protected internal Url ExchangeRatesEndpoint;
      protected internal Url PricesEndpoint;
      protected internal Url TimeEndpoint;
      protected internal Url NotificationsEndpoint;

      /// <summary>
      /// The main class for making Coinbase API calls.
      /// </summary>
      public CoinbaseApi(Config config = null)
      {
         if ( !string.IsNullOrWhiteSpace(config?.OAuthToken) )
         {
            this.oauthToken = config.OAuthToken;
            this.client = this.GetOAuthClient();
         }
         else if (!string.IsNullOrWhiteSpace(config?.ApiKey))
         {
            if (string.IsNullOrWhiteSpace(config?.ApiSecret)) throw new ArgumentException("The API secret must be specified.", nameof(apiSecret));

            this.apiKey = config.ApiKey;
            this.apiSecret = config.ApiSecret;

            this.client = this.CreateClient()
               .Configure(ApiKeyAuth);
         }

         this.useTimeApi = config?.UseTimeApi ?? true;
         this.apiUrl = config?.ApiUrl ?? Endpoint;
         this.AccountsEndpoint = apiUrl.AppendPathSegment("accounts");
         this.PaymentMethodsEndpoint = apiUrl.AppendPathSegment("payment-methods");
         this.CurrenciesEndpoint = apiUrl.AppendPathSegment("currencies");
         this.ExchangeRatesEndpoint = apiUrl.AppendPathSegment("exchange-rates");
         this.PricesEndpoint = apiUrl.AppendPathSegment("prices");
         this.TimeEndpoint = apiUrl.AppendPathSegment("time");
         this.NotificationsEndpoint = apiUrl.AppendPathSegment("notifications");
      }

      private void ApiKeyAuth(ClientFlurlHttpSettings config)
      {
         async Task SetHeaders(HttpCall http)
         {
            var body = http.RequestBody;
            var method = http.Request.Method.Method.ToUpperInvariant();
            var url = http.Request.RequestUri.PathAndQuery;

            string timestamp;
            if (useTimeApi)
            {
               var timeResult = await this.Data.GetCurrentTimeAsync().ConfigureAwait(false);
               timestamp = timeResult.Data.Epoch.ToString();
            }
            else
            {
               timestamp = ApiKeyAuthenticator.GetCurrentUnixTimestampSeconds().ToString(CultureInfo.CurrentCulture);
            }

            var signature = ApiKeyAuthenticator.GenerateSignature(timestamp, method, url, body, this.apiSecret).ToLower();

            http.FlurlRequest
               .WithHeader(HeaderNames.AccessKey, this.apiKey)
               .WithHeader(HeaderNames.AccessSign, signature)
               .WithHeader(HeaderNames.AccessTimestamp, timestamp);
         }

         config.BeforeCallAsync = SetHeaders;
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

      public virtual IFlurlClient GetOAuthClient()
      {
         return this.CreateClient()
            .WithOAuthBearerToken(oauthToken);
      }

      public virtual IFlurlClient GetApiKeyClient()
      {
         return null;
      }
      

      public void Dispose()
      {
         client?.Dispose();
      }
   }
}