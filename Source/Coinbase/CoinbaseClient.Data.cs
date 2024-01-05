﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;
using Newtonsoft.Json;

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
      async Task<Response<Money>> IDataEndpoint.GetBuyPriceAsync(string currencyPair, CancellationToken cancellationToken)
      {
         var responseBody = await Request(
            PricesEndpoint
               .AppendPathSegmentsRequire(currencyPair, "buy")
         ).GetStringAsync(
            cancellationToken: cancellationToken
         );
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new Response<Money>();

         return JsonConvert.DeserializeObject<Response<Money>>(responseBody);
      }

      /// <summary>
      /// Get the total price to sell one bitcoin or ether.
      /// Note that exchange rates fluctuates so the price is only correct for seconds at the time.This sell price includes standard Coinbase fee (1%) but excludes any other fees including bank fees.
      /// </summary>
      /// <param name="currencyPair">Currency pair such as BTC-USD, ETH-USD, etc.</param>
      async Task<Response<Money>> IDataEndpoint.GetSellPriceAsync(string currencyPair, CancellationToken cancellationToken)
      {
         var responseBody = await Request(
               PricesEndpoint
                  .AppendPathSegmentsRequire(currencyPair, "sell")
         ).GetStringAsync(
            cancellationToken: cancellationToken
         );
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new Response<Money>();

         return JsonConvert.DeserializeObject<Response<Money>>(responseBody);
      }

      /// <summary>
      /// Get the current market price for bitcoin. This is usually somewhere in between the buy and sell price.
      ///Note that exchange rates fluctuates so the price is only correct for seconds at the time.
      /// </summary>
      /// <param name="currencyPair"></param>
      /// <param name="cancellationToken"></param>
      async Task<Response<Money>> IDataEndpoint.GetSpotPriceAsync(string currencyPair, DateTime? date, CancellationToken cancellationToken)
      {
         /* added range checking to the date, to make sure it is between MinValue and MaxValue (exclusive) */
         var req = date is null || DateTime.MinValue.Equals(date) || DateTime.MaxValue.Equals(date)
            ? Request(PricesEndpoint.AppendPathSegmentsRequire(currencyPair, "spot"))
            : Request(
               PricesEndpoint.AppendPathSegmentsRequire(currencyPair, "spot")
                             .SetQueryParam("date", date.Value.ToString("yyyy-MM-dd"))
            );

         var responseBody = await req.GetStringAsync(cancellationToken: cancellationToken);
         if (string.IsNullOrWhiteSpace(responseBody))
            return new Response<Money>();

         return JsonConvert.DeserializeObject<Response<Money>>(responseBody);
      }

      /// <summary>
      /// Get current exchange rates. Default base currency is <c>USD</c>, but it can be defined as any supported currency. Returned rates will define the exchange rate for one unit of the base currency.
      /// </summary>
      /// <param name="currency">Base currency (default: USD)</param>
      async Task<Response<ExchangeRates>> IDataEndpoint.GetExchangeRatesAsync(string currency, CancellationToken cancellationToken)
      {
         var req = string.IsNullOrWhiteSpace(currency) ?
            Request(ExchangeRatesEndpoint)
            : Request(
               ExchangeRatesEndpoint
                  .SetQueryParam("currency", currency)
            );

         var responseBody = await req.GetStringAsync(cancellationToken: cancellationToken);
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new Response<ExchangeRates>();

         return JsonConvert.DeserializeObject<Response<ExchangeRates>>(responseBody);
      }

      /// <summary>
      /// List known currencies. Currency codes will conform to the ISO 4217 standard where possible. Currencies which have or had no representation in ISO 4217 may use a custom code (e.g. BTC).
      /// </summary>
      async Task<PagedResponse<Currency>> IDataEndpoint.GetCurrenciesAsync(PaginationOptions pagination, CancellationToken cancellationToken)
      {
         var responseBody = await Request(CurrenciesEndpoint.WithPagination(pagination))
            .GetStringAsync(cancellationToken: cancellationToken);
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new PagedResponse<Currency>();

         return JsonConvert.DeserializeObject<PagedResponse<Currency>>(responseBody);
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
            .GetJsonAsync<Response<Time>>(cancellationToken: cancellationToken);
      }
   }
}
