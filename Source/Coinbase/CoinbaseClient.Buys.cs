using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;

namespace Coinbase
{
   public interface IBuysEndpoint
   {
      /// <summary>
      /// Lists buys for an account.
      /// </summary>
      Task<PagedResponse<Buy>> ListBuysAsync(string accountId, PaginationOptions pagination = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// Get an individual buy.
      /// </summary>
      Task<Response<Buy>> GetBuyAsync(string accountId, string buyId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Buys a user-defined amount of bitcoin, bitcoin cash, litecoin or ethereum.
      /// There are two ways to define buy amounts–you can use either the amount or the total parameter:
      /// 1. When supplying amount, you’ll get the amount of bitcoin, bitcoin cash, litecoin or ethereum defined.With amount it’s recommended to use BTC or ETH as the currency value, but you can always specify a fiat currency and and the amount will be converted to BTC or ETH respectively.
      /// 2. When supplying total, your payment method will be debited the total amount and you’ll get the amount in BTC or ETH after fees have been reduced from the total.With total it’s recommended to use the currency of the payment method as the currency parameter, but you can always specify a different currency and it will be converted.
      /// Given the price of digital currency depends on the time of the call and on the amount of purchase, it’s recommended to use the commit: false parameter to create an uncommitted buy to show the confirmation for the user or get the final quote, and commit that with a separate request.
      /// If you need to query the buy price without locking in the buy, you can use quote: true option.This returns an unsaved buy and unlike commit: false, this buy can’t be completed.This option is useful when you need to show the detailed buy price quote for the user when they are filling a form or similar situation.
      /// </summary>
      Task<Response<Buy>> PlaceBuyOrderAsync(string accountId, PlaceBuy placeBuy, CancellationToken cancellationToken = default);

      /// <summary>
      /// Completes a buy that is created in commit: false state.
      /// If the exchange rate has changed since the buy was created, this call will fail with the error “The exchange rate updated while you were waiting.The new total is shown below”.
      /// The buy’s total will also be updated.You can repeat the /commit call to accept the new values and start the buy at the new rates.
      /// </summary>
      Task<Response<Buy>> CommitBuyAsync(string accountId, string buyId, CancellationToken cancellationToken = default);
   }

   public partial class CoinbaseClient : IBuysEndpoint
   {
      public IBuysEndpoint Buys => this;

      /// <summary>
      /// Lists buys for an account.
      /// </summary>
      Task<PagedResponse<Buy>> IBuysEndpoint.ListBuysAsync(string accountId, PaginationOptions pagination, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "buys")
            .WithPagination(pagination)
            .WithClient(this)
            .GetJsonAsync<PagedResponse<Buy>>(cancellationToken);
      }

      /// <summary>
      /// Get an individual buy.
      /// </summary>
      Task<Response<Buy>> IBuysEndpoint.GetBuyAsync(string accountId, string buyId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "buys", buyId)
            .WithClient(this)
            .GetJsonAsync<Response<Buy>>(cancellationToken);
      }

      /// <summary>
      /// Buys a user-defined amount of bitcoin, bitcoin cash, litecoin or ethereum.
      /// There are two ways to define buy amounts–you can use either the amount or the total parameter:
      /// 1. When supplying amount, you’ll get the amount of bitcoin, bitcoin cash, litecoin or ethereum defined.With amount it’s recommended to use BTC or ETH as the currency value, but you can always specify a fiat currency and and the amount will be converted to BTC or ETH respectively.
      /// 2. When supplying total, your payment method will be debited the total amount and you’ll get the amount in BTC or ETH after fees have been reduced from the total.With total it’s recommended to use the currency of the payment method as the currency parameter, but you can always specify a different currency and it will be converted.
      /// Given the price of digital currency depends on the time of the call and on the amount of purchase, it’s recommended to use the commit: false parameter to create an uncommitted buy to show the confirmation for the user or get the final quote, and commit that with a separate request.
      /// If you need to query the buy price without locking in the buy, you can use quote: true option.This returns an unsaved buy and unlike commit: false, this buy can’t be completed.This option is useful when you need to show the detailed buy price quote for the user when they are filling a form or similar situation.
      /// </summary>
      Task<Response<Buy>> IBuysEndpoint.PlaceBuyOrderAsync(string accountId, PlaceBuy placeBuy, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "buys")
            .WithClient(this)
            .PostJsonAsync(placeBuy, cancellationToken)
            .ReceiveJson<Response<Buy>>();
      }


      /// <summary>
      /// Completes a buy that is created in commit: false state.
      /// If the exchange rate has changed since the buy was created, this call will fail with the error “The exchange rate updated while you were waiting.The new total is shown below”.
      /// The buy’s total will also be updated.You can repeat the /commit call to accept the new values and start the buy at the new rates.
      /// </summary>
      Task<Response<Buy>> IBuysEndpoint.CommitBuyAsync(string accountId, string buyId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "buys", buyId, "commit")
            .WithClient(this)
            .PostJsonAsync(null, cancellationToken)
            .ReceiveJson<Response<Buy>>();
      }


   }
}
