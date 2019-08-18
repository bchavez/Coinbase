using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;

namespace Coinbase
{

   public interface IDataEndpoint
   {
      /// <summary>
      /// Get the total price to buy one bitcoin or ether.
      /// Note that exchange rates fluctuates so the price is only correct for seconds at the time.This buy price includes standard Coinbase fee (1%) but excludes any other fees including bank fees.
      /// </summary>
      /// <param name="currencyPair">Currency pair such as BTC-USD, ETH-USD, etc.</param>
      Task<Response<Money>> GetBuyPriceAsync(string currencyPair, CancellationToken cancellationToken = default);

      /// <summary>
      /// Get the total price to sell one bitcoin or ether.
      /// Note that exchange rates fluctuates so the price is only correct for seconds at the time.This sell price includes standard Coinbase fee (1%) but excludes any other fees including bank fees.
      /// </summary>
      /// <param name="currencyPair">Currency pair such as BTC-USD, ETH-USD, etc.</param>
      Task<Response<Money>> GetSellPriceAsync(string currencyPair, CancellationToken cancellationToken = default);

      /// <summary>
      /// Get the current market price for bitcoin. This is usually somewhere in between the buy and sell price.
      ///Note that exchange rates fluctuates so the price is only correct for seconds at the time.
      /// </summary>
      /// <param name="currencyPair"></param>
      /// <param name="cancellationToken"></param>
      Task<Response<Money>> GetSpotPriceAsync(string currencyPair, DateTime? date = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// Get current exchange rates. Default base currency is USD but it can be defined as any supported currency. Returned rates will define the exchange rate for one unit of the base currency.
      /// </summary>
      /// <param name="currency">Base currency (default: USD)</param>
      Task<Response<ExchangeRates>> GetExchangeRatesAsync(string currency = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// List known currencies. Currency codes will conform to the ISO 4217 standard where possible. Currencies which have or had no representation in ISO 4217 may use a custom code (e.g. BTC).
      /// </summary>
      Task<PagedResponse<Currency>> GetCurrenciesAsync(PaginationOptions pagination = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// Get the API server time.
      /// </summary>
      Task<Response<Time>> GetCurrentTimeAsync(CancellationToken cancellationToken = default);
   }

   public partial class CoinbaseClient : IDataEndpoint
   {
      public IDataEndpoint Data => this;

      /// <summary>
      /// Get the total price to buy one bitcoin or ether.
      /// Note that exchange rates fluctuates so the price is only correct for seconds at the time.This buy price includes standard Coinbase fee (1%) but excludes any other fees including bank fees.
      /// </summary>
      /// <param name="currencyPair">Currency pair such as BTC-USD, ETH-USD, etc.</param>
      Task<Response<Money>> IDataEndpoint.GetBuyPriceAsync(string currencyPair, CancellationToken cancellationToken)
      {
         return this.PricesEndpoint
            .WithClient(this)
            .AppendPathSegmentsRequire(currencyPair, "buy")
            .GetJsonAsync<Response<Money>>(cancellationToken);
      }

      /// <summary>
      /// Get the total price to sell one bitcoin or ether.
      /// Note that exchange rates fluctuates so the price is only correct for seconds at the time.This sell price includes standard Coinbase fee (1%) but excludes any other fees including bank fees.
      /// </summary>
      /// <param name="currencyPair">Currency pair such as BTC-USD, ETH-USD, etc.</param>
      Task<Response<Money>> IDataEndpoint.GetSellPriceAsync(string currencyPair, CancellationToken cancellationToken)
      {
         return this.PricesEndpoint
            .WithClient(this)
            .AppendPathSegmentsRequire(currencyPair, "sell")
            .GetJsonAsync<Response<Money>>(cancellationToken);
      }

      /// <summary>
      /// Get the current market price for bitcoin. This is usually somewhere in between the buy and sell price.
      ///Note that exchange rates fluctuates so the price is only correct for seconds at the time.
      /// </summary>
      /// <param name="currencyPair"></param>
      /// <param name="cancellationToken"></param>
      Task<Response<Money>> IDataEndpoint.GetSpotPriceAsync(string currencyPair, DateTime? date, CancellationToken cancellationToken)
      {
         var req = this.PricesEndpoint
            .WithClient(this)
            .AppendPathSegmentsRequire(currencyPair, "spot");

         if (!(date is null))
         {
            req = req.SetQueryParam("date", date.Value.ToString("yyyy-MM-dd"));
         }

         return req.GetJsonAsync<Response<Money>>(cancellationToken);
      }

      /// <summary>
      /// Get current exchange rates. Default base currency is USD but it can be defined as any supported currency. Returned rates will define the exchange rate for one unit of the base currency.
      /// </summary>
      /// <param name="currency">Base currency (default: USD)</param>
      Task<Response<ExchangeRates>> IDataEndpoint.GetExchangeRatesAsync(string currency, CancellationToken cancellationToken)
      {
         var req = this.ExchangeRatesEndpoint
            .WithClient(this);

         if (!(currency is null))
         {
            req.SetQueryParam("currency", currency);
         }

         return req.GetJsonAsync<Response<ExchangeRates>>(cancellationToken);
      }

      /// <summary>
      /// List known currencies. Currency codes will conform to the ISO 4217 standard where possible. Currencies which have or had no representation in ISO 4217 may use a custom code (e.g. BTC).
      /// </summary>
      Task<PagedResponse<Currency>> IDataEndpoint.GetCurrenciesAsync(PaginationOptions pagination, CancellationToken cancellationToken)
      {
         return this.CurrenciesEndpoint
            .WithPagination(pagination)
            .WithClient(this)
            .GetJsonAsync<PagedResponse<Currency>>(cancellationToken);
      }


      /// <summary>
      /// Get the API server time.
      /// </summary>
      Task<Response<Time>> IDataEndpoint.GetCurrentTimeAsync(CancellationToken cancellationToken)
      {
         // Manually make this request outside the scope of .WithClient(this)
         // because when UseTimeApi =true and when the user makes the first request,
         // we need to know the server time but if we use .WithClient(this),
         // we'll get stuck in a recursive situation with no base case
         // to resolve the actual server time. So, we have to make this
         // request out of scope of this configured client.
         return this.TimeEndpoint
            .WithHeader(HeaderNames.Version, ApiVersionDate)
            .WithHeader("User-Agent", UserAgent)
            .GetJsonAsync<Response<Time>>(cancellationToken);
      }

   }
}
