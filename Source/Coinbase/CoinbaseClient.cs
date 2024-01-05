using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
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
      IDataEndpoint Data { get; }
      IDepositsEndpoint Deposits { get; }
      INotificationsEndpoint Notifications { get; }
      IPaymentMethodsEndpoint PaymentMethods { get; }
      ITransactionsEndpoint Transactions { get; }
      IWithdrawalsEndpoint Withdrawals { get; }
   }

   public partial class CoinbaseClient : FlurlClient, ICoinbaseClient
   {
      public const string ApiVersionDate = "2021-06-05";

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

      /// <summary>
      /// Enable HTTP debugging via Fiddler. Ensure Tools > Fiddler Options... > Connections is enabled and has a port configured.
      /// Then, call this method with the following URL format: http://localhost.:PORT where PORT is the port number Fiddler proxy
      /// is listening on. (Be sure to include the period after the localhost).
      /// </summary>
      /// <param name="proxyUrl">The full proxy URL Fiddler proxy is listening on. IE: http://localhost.:8888 - The period after localhost is important to include.</param>
      public void EnableFiddlerDebugProxy(string proxyUrl)
      {
         var webProxy = new WebProxy(proxyUrl, BypassOnLocal: false);

         FlurlHttp.Clients.WithDefaults(builder => builder.ConfigureInnerHandler(
            hch =>
               {
                  hch.Proxy = new WebProxy(proxyUrl, BypassOnLocal: false);
                  hch.UseProxy = true;
               }
         ));
      }

      /// <summary>
      /// Get the next page of data given the current paginated response.
      /// </summary>
      /// <param name="currentPage">The current paged response.</param>
      /// <returns>The next page of data.</returns>
      public Task<PagedResponse<T>> GetNextPageAsync<T>(PagedResponse<T> currentPage, CancellationToken cancellationToken = default)
      {
         if( !currentPage.HasNextPage() ) throw new NullReferenceException("No next page.");

         return GetPageAsync<T>(currentPage.Pagination.NextUri, cancellationToken);
      }

      ///// <summary>
      ///// Get the previous page of data given the current pagination response.
      ///// </summary>
      ///// <param name="currentPage">The current paged response.</param>
      ///// <returns>The previous page of data.</returns>
      //public Task<PagedResponse<T>> PreviousPageAsync<T>(PagedResponse<T> currentPage, CancellationToken cancellationToken = default)
      //{
      //   if( !currentPage.HasPrevPage() ) throw new NullReferenceException("No previous page.");

      //   return GetPageAsync<T>(currentPage.Pagination.PreviousUri, cancellationToken);
      //}

      /// <summary>
      /// Internally used for getting a next or previous page.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="pageUrl"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      protected internal Task<PagedResponse<T>> GetPageAsync<T>(string pageUrl, CancellationToken cancellationToken = default)
      {
         pageUrl = pageUrl.Remove(0, 4);
         return this.Request(Config.ApiUrl + pageUrl)
             .GetJsonAsync<PagedResponse<T>>(cancellationToken: cancellationToken);
      }

      /// <summary>
      /// Captures the low-level <seealso cref="HttpResponseMessage" /> from a
      /// underlying request. Useful in advanced scenarios where you
      /// want to check HTTP headers, HTTP status code or
      /// inspect the response body manually.
      /// </summary>
      /// <param name="responseGetter">A function that must be called to
      /// retrieve the <seealso cref="HttpResponseMessage"/>
      /// </param>
      /// <returns>Returns the <seealso cref="HttpResponseMessage"/> of the
      /// underlying HTTP request.</returns>
      public CoinbaseClient HoistResponse(out Func<IFlurlResponse> responseGetter)
      {
         IFlurlResponse msg = null;

         this.WithSettings(_ => this.AfterCall(call => msg = call.Response));

         responseGetter = () => msg;
         return this;
      }
   }
}
