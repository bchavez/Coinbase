using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;

namespace Coinbase
{
   public interface ISellsEndpoint
   {
      /// <summary>
      /// Lists sells for an account.
      /// </summary>
      Task<PagedResponse<Sell>> ListSellsAsync(string accountId, PaginationOptions pagination = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// Get an individual sell.
      /// </summary>
      Task<Response<Sell>> GetSellAsync(string accountId, string sellId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Sells a user-defined amount of bitcoin, bitcoin cash, litecoin or ethereum.
      /// There are two ways to define sell amounts–you can use either the amount or the total parameter:
      /// 1.When supplying amount, you’ll get the amount of bitcoin, bitcoin cash, litecoin or ethereum defined. With amount it’s recommended to use BTC or ETH as the currency value, but you can always specify a fiat currency and the amount will be converted to BTC or ETH respectively.
      /// 2.When supplying total, your payment method will be credited the total amount and you’ll get the amount in BTC or ETH after fees have been reduced from the subtotal. With total it’s recommended to use the currency of the payment method as the currency parameter, but you can always specify a different currency and it will be converted.
      /// Given the price of digital currency depends on the time of the call and amount of the sell, it’s recommended to use the commit: false parameter to create an uncommitted sell to get a quote and then to commit that with a separate request.
      ///If you need to query the sell price without locking in the sell, you can use quote: true option. This returns an unsaved sell and unlike commit: false, this sell can’t be completed. This option is useful when you need to show the detailed sell price quote for the user when they are filling a form or similar situation.
      /// </summary>
      Task<Response<Sell>> PlaceSellOrderAsync(string accountId, PlaceSell placeSell, CancellationToken cancellationToken = default);

      /// <summary>
      /// Completes a sell that is created in commit: false state.
      /// If the exchange rate has changed since the sell was created, this call will fail with the error “The exchange rate updated while you were waiting.The new total is shown below”.
      /// The sell’s total will also be updated.You can repeat the /commit call to accept the new values and commit the sell at the new rates.
      /// </summary>
      Task<Response<Sell>> CommitSellAsync(string accountId, string sellId, CancellationToken cancellationToken = default);
   }

   public partial class CoinbaseClient : ISellsEndpoint
   {
      public ISellsEndpoint Sells => this;

      /// <summary>
      /// Lists sells for an account.
      /// </summary>
      Task<PagedResponse<Sell>> ISellsEndpoint.ListSellsAsync(string accountId, PaginationOptions pagination, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "sells")
            .WithPagination(pagination)
            .WithClient(this)
            .GetJsonAsync<PagedResponse<Sell>>(cancellationToken);
      }

      /// <summary>
      /// Get an individual sell.
      /// </summary>
      Task<Response<Sell>> ISellsEndpoint.GetSellAsync(string accountId, string sellId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "sells", sellId)
            .WithClient(this)
            .GetJsonAsync<Response<Sell>>(cancellationToken);
      }

      /// <summary>
      /// Sells a user-defined amount of bitcoin, bitcoin cash, litecoin or ethereum.
      /// There are two ways to define sell amounts–you can use either the amount or the total parameter:
      /// 1.When supplying amount, you’ll get the amount of bitcoin, bitcoin cash, litecoin or ethereum defined. With amount it’s recommended to use BTC or ETH as the currency value, but you can always specify a fiat currency and the amount will be converted to BTC or ETH respectively.
      /// 2.When supplying total, your payment method will be credited the total amount and you’ll get the amount in BTC or ETH after fees have been reduced from the subtotal. With total it’s recommended to use the currency of the payment method as the currency parameter, but you can always specify a different currency and it will be converted.
      /// Given the price of digital currency depends on the time of the call and amount of the sell, it’s recommended to use the commit: false parameter to create an uncommitted sell to get a quote and then to commit that with a separate request.
      ///If you need to query the sell price without locking in the sell, you can use quote: true option. This returns an unsaved sell and unlike commit: false, this sell can’t be completed. This option is useful when you need to show the detailed sell price quote for the user when they are filling a form or similar situation.
      /// </summary>
      Task<Response<Sell>> ISellsEndpoint.PlaceSellOrderAsync(string accountId, PlaceSell placeSell, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "sells")
            .WithClient(this)
            .PostJsonAsync(placeSell, cancellationToken)
            .ReceiveJson<Response<Sell>>();
      }

      /// <summary>
      /// Completes a sell that is created in commit: false state.
      /// If the exchange rate has changed since the sell was created, this call will fail with the error “The exchange rate updated while you were waiting.The new total is shown below”.
      /// The sell’s total will also be updated.You can repeat the /commit call to accept the new values and commit the sell at the new rates.
      /// </summary>
      Task<Response<Sell>> ISellsEndpoint.CommitSellAsync(string accountId, string sellId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "sells", sellId, "commit")
            .WithClient(this)
            .PostJsonAsync(null, cancellationToken)
            .ReceiveJson<Response<Sell>>();
      }



   }
}
