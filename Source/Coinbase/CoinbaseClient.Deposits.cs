using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;

namespace Coinbase
{

   public interface IDepositsEndpoint
   {
      /// <summary>
      /// Lists deposits for an account.
      /// </summary>
      Task<PagedResponse<Deposit>> ListDepositsAsync(string accountId, PaginationOptions pagination = null, CancellationToken cancellationToken = default);
      /// <summary>
      /// Show an individual deposit.
      /// </summary>
      Task<Response<Deposit>> GetDepositAsync(string accountId, string depositId, CancellationToken cancellationToken = default);
      /// <summary>
      /// Deposits user-defined amount of funds to a fiat account.
      /// </summary>
      Task<Response<Deposit>> DepositFundsAsync(string accountId, DepositFunds depositFunds, CancellationToken cancellationToken = default);
      /// <summary>
      /// Completes a deposit that is created in commit: false state
      /// </summary>
      Task<Response<Deposit>> CommitDepositAsync(string accountId, string depositId, CancellationToken cancellationToken = default);
   }

   public partial class CoinbaseClient : IDepositsEndpoint
   {
      public IDepositsEndpoint Deposits => this;

      private const string deposits = "deposits";

      /// <inheritdoc />
      Task<PagedResponse<Deposit>> IDepositsEndpoint.ListDepositsAsync(string accountId, PaginationOptions pagination, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, deposits)
            .WithPagination(pagination)
            .WithClient(this)
            .GetJsonAsync<PagedResponse<Deposit>>(cancellationToken);
      }

      /// <inheritdoc />
      Task<Response<Deposit>> IDepositsEndpoint.GetDepositAsync(string accountId, string depositId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, deposits, depositId)
            .WithClient(this)
            .GetJsonAsync<Response<Deposit>>(cancellationToken);
      }

      /// <inheritdoc />
      Task<Response<Deposit>> IDepositsEndpoint.DepositFundsAsync(string accountId, DepositFunds depositFunds, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, deposits)
            .WithClient(this)
            .PostJsonAsync(depositFunds, cancellationToken)
            .ReceiveJson<Response<Deposit>>();
      }

      /// <inheritdoc />
      Task<Response<Deposit>> IDepositsEndpoint.CommitDepositAsync(string accountId, string depositId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, deposits, depositId, "commit")
            .WithClient(this)
            .PostJsonAsync(null, cancellationToken)
            .ReceiveJson<Response<Deposit>>();
      }
   }
}
