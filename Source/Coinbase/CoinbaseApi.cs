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

   public interface ICoinbaseOAuthClient : ICoinbaseClient
   {
   }

   public interface ICoinbaseApiClient : ICoinbaseClient
   {
   }

   public partial class CoinbaseOAuthApi : CoinbaseApiBase<OAuthConfig>, ICoinbaseOAuthClient
   {
      public CoinbaseOAuthApi(OAuthConfig config) : base(config)
      {
      }

      protected internal override void ConfigureClient()
      {
         this.WithOAuthBearerToken(this.config.OAuthToken);
      }
   }

   public partial class CoinbaseApi : CoinbaseApiBase<ApiKeyConfig>, ICoinbaseApiClient
   {
      public CoinbaseApi(ApiKeyConfig config) : base(config)
      {
      }

      protected internal override void ConfigureClient()
      {
          this.Configure(settings => ApiKeyAuth(settings, this.config));
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
   }

   public class PublicCoinbaseApi : CoinbaseApiBase
   {
      //TODO: Ideally this should be a seperate a sub set of the coinbase api 
      //which only exposes the unauthenticated endpoints
      public PublicCoinbaseApi() : base(new Config())
      {
      }
   }

   public partial class CoinbaseApiBase : FlurlClient, ICoinbaseClient
   {
      public const string ApiVersionDate = "2017-08-07";
      public const string Endpoint = "https://api.coinbase.com/v2/";

      private Config _config;

      //This effectively prevents anyone from getting the _configs out of sync,
      //If we were to allow null in this constructor We would have to initialize the private config here,
      //and the TConfig from the parent with different instances, since this must be called with a value
      //If someone updates a property on the Config in the super it will be updated here.
      internal CoinbaseApiBase(Config config)
      {
         if (config == null)
         {
            throw new ArgumentException("Provide a coinbase configuration, if you're trying to call public methods use the PublicCoinbaseApi");
         }
         this._config = config;
      }

      internal static readonly string UserAgent =
           $"{AssemblyVersionInformation.AssemblyProduct}/{AssemblyVersionInformation.AssemblyVersion} ({AssemblyVersionInformation.AssemblyTitle}; {AssemblyVersionInformation.AssemblyDescription})";

      protected internal Url AccountsEndpoint => this._config.ApiUrl.AppendPathSegment("accounts");
      protected internal Url PaymentMethodsEndpoint => this._config.ApiUrl.AppendPathSegment("payment-methods");
      protected internal Url CurrenciesEndpoint => this._config.ApiUrl.AppendPathSegment("currencies");
      protected internal Url ExchangeRatesEndpoint => this._config.ApiUrl.AppendPathSegment("exchange-rates");
      protected internal Url PricesEndpoint => this._config.ApiUrl.AppendPathSegment("prices");
      protected internal Url TimeEndpoint => this._config.ApiUrl.AppendPathSegment("time");
      protected internal Url NotificationsEndpoint => this._config.ApiUrl.AppendPathSegment("notifications");

    }

   public partial class CoinbaseApiBase<TConfig> : CoinbaseApiBase
      where TConfig : Config, new()
   {
      protected internal readonly TConfig config;

      /// <summary>
      /// The main class for making Coinbase API calls.
      /// </summary>
      public CoinbaseApiBase(TConfig config) : base(config)
      {
         this.config = config;
         this.config.EnsureValid();

         this.WithHeader(HeaderNames.Version, ApiVersionDate)
            .WithHeader("User-Agent", UserAgent)
            .AllowAnyHttpStatus(); //Issue 33

         this.ConfigureClient();
      }

      protected internal virtual void ConfigureClient()
      {
      }  
   }
}