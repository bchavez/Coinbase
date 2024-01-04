using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;

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

      Task<PagedResponse<Withdrawal>> IWithdrawalsEndpoint.ListWithdrawalsAsync(string accountId, PaginationOptions pagination, CancellationToken cancellationToken)
      {
         return Request(
               AccountsEndpoint.AppendPathSegmentsRequire(accountId, withdrawals)
                               .WithPagination(pagination)
            )
            .GetJsonAsync<PagedResponse<Withdrawal>>(cancellationToken: cancellationToken);
      }

      Task<Response<Withdrawal>> IWithdrawalsEndpoint.GetWithdrawalAsync(string accountId, string withdrawalId, CancellationToken cancellationToken)
      {
         return Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, withdrawals, withdrawalId))
            .GetJsonAsync<Response<Withdrawal>>(cancellationToken: cancellationToken);
      }

      Task<Response<Withdrawal>> IWithdrawalsEndpoint.WithdrawalFundsAsync(string accountId, WithdrawalFunds withdrawalFunds, CancellationToken cancellationToken)
      {
         return Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, withdrawals))
                .PostJsonAsync(withdrawalFunds, cancellationToken: cancellationToken)
                .ReceiveJson<Response<Withdrawal>>();
      }

      Task<Response<Withdrawal>> IWithdrawalsEndpoint.CommitWithdrawalAsync(string accountId, string withdrawalId, CancellationToken cancellationToken)
      {
         return Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, withdrawals, withdrawalId, "commit"))
                .PostJsonAsync(null, cancellationToken: cancellationToken)
                .ReceiveJson<Response<Withdrawal>>();
      }
   }
}
