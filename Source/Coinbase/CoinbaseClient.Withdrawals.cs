using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;
using Newtonsoft.Json;

namespace Coinbase
{
   public interface IWithdrawalsEndpoint
   {
      /// <summary>
      /// Lists deposits for an account.
      /// </summary>
      Task<PagedResponse<Withdrawal>> ListWithdrawalsAsync(string accountId, PaginationOptions pagination = null, CancellationToken cancellationToken = default);
      /// <summary>
      /// Show an individual deposit.
      /// </summary>
      Task<Response<Withdrawal>> GetWithdrawalAsync(string accountId, string withdrawalId, CancellationToken cancellationToken = default);
      /// <summary>
      /// Deposits user-defined amount of funds to a fiat account.
      /// </summary>
      Task<Response<Withdrawal>> WithdrawalFundsAsync(string accountId, WithdrawalFunds withdrawalFunds, CancellationToken cancellationToken = default);
      /// <summary>
      /// Completes a deposit that is created in commit: false state
      /// </summary>
      Task<Response<Withdrawal>> CommitWithdrawalAsync(string accountId, string withdrawalId, CancellationToken cancellationToken = default);
   }
   
   public partial class CoinbaseClient : IWithdrawalsEndpoint
   {
      public IWithdrawalsEndpoint Withdrawals => this;

      private const string withdrawals = "withdrawals";

      async Task<PagedResponse<Withdrawal>> IWithdrawalsEndpoint.ListWithdrawalsAsync(string accountId, PaginationOptions pagination, CancellationToken cancellationToken)
      {
         var responseBody = await Request(
               AccountsEndpoint.AppendPathSegmentsRequire(accountId, withdrawals)
                               .WithPagination(pagination)
            )
            .GetStringAsync(cancellationToken: cancellationToken);
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new PagedResponse<Withdrawal>();

         return JsonConvert.DeserializeObject<PagedResponse<Withdrawal>>(responseBody);
      }

      async Task<Response<Withdrawal>> IWithdrawalsEndpoint.GetWithdrawalAsync(string accountId, string withdrawalId, CancellationToken cancellationToken)
      {
         var responseBody = await Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, withdrawals, withdrawalId))
            .GetStringAsync(cancellationToken: cancellationToken);
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new Response<Withdrawal>();

         return JsonConvert.DeserializeObject<Response<Withdrawal>>(responseBody);
      }

      async Task<Response<Withdrawal>> IWithdrawalsEndpoint.WithdrawalFundsAsync(string accountId, WithdrawalFunds withdrawalFunds, CancellationToken cancellationToken)
      {
         using var response = Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, withdrawals))
                .PostJsonAsync(withdrawalFunds, cancellationToken: cancellationToken);
         var responseBody = await response.ReceiveString();
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new Response<Withdrawal>();

         return JsonConvert.DeserializeObject<Response<Withdrawal>>(responseBody);
      }

      async Task<Response<Withdrawal>> IWithdrawalsEndpoint.CommitWithdrawalAsync(string accountId, string withdrawalId, CancellationToken cancellationToken)
      {
         using var response = Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, withdrawals, withdrawalId, "commit"))
                .PostJsonAsync(null, cancellationToken: cancellationToken);
         var responseBody = await response.ReceiveString();
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new Response<Withdrawal>();

         return JsonConvert.DeserializeObject<Response<Withdrawal>>(responseBody);
      }
   }
}
