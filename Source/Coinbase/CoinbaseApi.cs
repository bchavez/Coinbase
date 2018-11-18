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

   public partial class CoinbaseApi : FlurlClient, ICoinbaseClient
   {
      public const string ApiVersionDate = "2017-08-07";

      protected internal readonly Config config;

      public const string Endpoint = "https://api.coinbase.com/v2/";
      public const string TokenEndpoint = "https://api.coinbase.com/oauth/token";

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

      private void OAuthConfiguration(ClientFlurlHttpSettings client, OAuthConfig oauthConfig)
      {
            async Task ApplyAuthorization(HttpCall call)
            {
               call.FlurlRequest.WithOAuthBearerToken(oauthConfig.OAuthToken);
            }
         client.BeforeCallAsync = ApplyAuthorization;
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

      public const string EXPIRED_TOKEN = "expired_token";
      public async Task HandleErrorAsync(HttpCall call)
      {
            var exception = call.Exception;
            if (exception is FlurlHttpException)
            {
                FlurlHttpException ex = (exception as FlurlHttpException);
                var errorResponse = await ex.GetResponseJsonAsync<ErrorResponse>();
                if (errorResponse.Errors.Any(x => x.Id == EXPIRED_TOKEN))
                {
                    var tokenResponse = await this.RefreshOAuthToken();
                    call.Response = await call.FlurlRequest.SendAsync(call.Request.Method, call.Request.Content);
                    call.ExceptionHandled = true;
                }
            }
      }

      public async Task<RefreshResponse> RefreshOAuthToken(CancellationToken cancellationToken = default)
      {
          var oauthConfig = (this.config as OAuthConfig);

            var data = new {
                refresh_token = oauthConfig.RefreshToken,
                grant_type = "refresh_token"
            };

            var response = await oauthConfig.TokenEndpoint.WithClient(this)
                .PostJsonAsync(data, cancellationToken)
                .ReceiveJson<RefreshResponse>();

            oauthConfig.RefreshToken = response.RefreshToken;
            oauthConfig.OAuthToken = response.AccessToken;

            return response;
      }     

      protected internal virtual void ConfigureClient()
      {
         this.WithHeader(HeaderNames.Version, ApiVersionDate)
              .WithHeader("User-Agent", UserAgent);
         
         this.Configure(x =>{
            x.OnErrorAsync = HandleErrorAsync;
         });

         if (this.config is OAuthConfig oauth)
         {
            this.Configure(settings => OAuthConfiguration(settings, oauth));
         }
         if (this.config is ApiKeyConfig key)
         {
            this.Configure(settings => ApiKeyAuth(settings, key));
         }
      }  
   }
}