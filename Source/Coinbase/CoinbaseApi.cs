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
               var timeResult = await this.GetCurrentTimeAsync().ConfigureAwait(false);
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



      /// <summary>
      /// Get the total price to buy one bitcoin or ether.
      /// Note that exchange rates fluctuates so the price is only correct for seconds at the time.This buy price includes standard Coinbase fee (1%) but excludes any other fees including bank fees.
      /// </summary>
      /// <param name="currencyPair">Currency pair such as BTC-USD, ETH-USD, etc.</param>
      public virtual Task<Response<Money>> GetBuyPriceAsync(string currencyPair, CancellationToken cancellationToken = default)
      {
         return this.PricesEndpoint
            .AppendPathSegments(currencyPair, "buy")
            .GetJsonAsync<Response<Money>>(cancellationToken);
      }

      /// <summary>
      /// Get the total price to sell one bitcoin or ether.
      /// Note that exchange rates fluctuates so the price is only correct for seconds at the time.This sell price includes standard Coinbase fee (1%) but excludes any other fees including bank fees.
      /// </summary>
      /// <param name="currencyPair">Currency pair such as BTC-USD, ETH-USD, etc.</param>
      public virtual Task<Response<Money>> GetSellPriceAsync(string currencyPair, CancellationToken cancellationToken = default)
      {
         return this.PricesEndpoint
            .AppendPathSegments(currencyPair, "sell")
            .GetJsonAsync<Response<Money>>(cancellationToken);
      }

      /// <summary>
      /// Get the current market price for bitcoin. This is usually somewhere in between the buy and sell price.
      ///Note that exchange rates fluctuates so the price is only correct for seconds at the time.
      /// </summary>
      /// <param name="currencyPair"></param>
      /// <param name="cancellationToken"></param>
      public virtual Task<Response<Money>> GetSpotPriceAsync(string currencyPair, DateTime? date = null, CancellationToken cancellationToken = default)
      {
         var req =this.PricesEndpoint
            .AppendPathSegments(currencyPair, "spot");

         if( !(date is null) )
         {
            req = req.SetQueryParam("date", date.Value.ToString("yyyy-MM-dd"));
         }

         return req.GetJsonAsync<Response<Money>>(cancellationToken);
      }

      /// <summary>
      /// Get current exchange rates. Default base currency is USD but it can be defined as any supported currency. Returned rates will define the exchange rate for one unit of the base currency.
      /// </summary>
      /// <param name="currency">Base currency (default: USD)</param>
      public virtual Task<Response<ExchangeRates>> GetExchangeRatesAsync(string currency = null, CancellationToken cancellationToken = default)
      {
         var req = this.ExchangeRatesEndpoint;

         if( !(currency is null) )
         {
            req.SetQueryParam("currency", currency);
         }

         return req.GetJsonAsync<Response<ExchangeRates>>(cancellationToken);
      }

      /// <summary>
      /// List known currencies. Currency codes will conform to the ISO 4217 standard where possible. Currencies which have or had no representation in ISO 4217 may use a custom code (e.g. BTC).
      /// </summary>
      public virtual Task<PagedResponse<Currency>> GetCurrenciesAsync()
      {
         return this.CurrenciesEndpoint.GetJsonAsync<PagedResponse<Currency>>();
      }


      /// <summary>
      /// Get the API server time.
      /// </summary>
      public virtual Task<Response<Time>> GetCurrentTimeAsync(CancellationToken cancellationToken = default)
      {
         return this.TimeEndpoint.GetJsonAsync<Response<Time>>(cancellationToken);
      }

      public void Dispose()
      {
         client?.Dispose();
      }
   }
}