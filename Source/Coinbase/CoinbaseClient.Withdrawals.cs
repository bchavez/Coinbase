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
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, withdrawals)
            .WithPagination(pagination)
            .WithClient(this)
            .GetJsonAsync<PagedResponse<Withdrawal>>(cancellationToken);
      }

      Task<Response<Withdrawal>> IWithdrawalsEndpoint.GetWithdrawalAsync(string accountId, string withdrawalId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, withdrawals, withdrawalId)
            .WithClient(this)
            .GetJsonAsync<Response<Withdrawal>>(cancellationToken);
      }

      Task<Response<Withdrawal>> IWithdrawalsEndpoint.WithdrawalFundsAsync(string accountId, WithdrawalFunds withdrawalFunds, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, withdrawals)
            .WithClient(this)
            .PostJsonAsync(withdrawalFunds, cancellationToken)
            .ReceiveJson<Response<Withdrawal>>();

      }

      Task<Response<Withdrawal>> IWithdrawalsEndpoint.CommitWithdrawalAsync(string accountId, string withdrawalId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, withdrawals, withdrawalId, "commit")
            .WithClient(this)
            .PostJsonAsync(null, cancellationToken)
            .ReceiveJson<Response<Withdrawal>>();
      }
   }
}
