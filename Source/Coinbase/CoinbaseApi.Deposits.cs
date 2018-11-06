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
      Task<PagedResponse<Deposit>> ListDepositsAsync(string accountId, CancellationToken cancellationToken = default);
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
      /// </summary
      Task<Response<Deposit>> CommitDepositAsync(string accountId, string depositId, CancellationToken cancellationToken = default);
   }

   public partial class CoinbaseApi : IDepositsEndpoint
   {
      public IDepositsEndpoint Deposits => this;

      private const string deposits = "deposits";

      /// <inheritdoc />
      Task<PagedResponse<Deposit>> IDepositsEndpoint.ListDepositsAsync(string accountId, CancellationToken cancellationToken = default)
      {
         return this.AccountsEndpoint
            .AppendPathSegments(accountId, deposits)
            .WithClient(this.client)
            .GetJsonAsync<PagedResponse<Deposit>>(cancellationToken);
      }

      /// <inheritdoc />
      Task<Response<Deposit>> IDepositsEndpoint.GetDepositAsync(string accountId, string depositId, CancellationToken cancellationToken = default)
      {
         return this.AccountsEndpoint
            .AppendPathSegments(accountId, deposits, depositId)
            .WithClient(this.client)
            .GetJsonAsync<Response<Deposit>>(cancellationToken);
      }

      /// <inheritdoc />
      Task<Response<Deposit>> IDepositsEndpoint.DepositFundsAsync(string accountId, DepositFunds depositFunds, CancellationToken cancellationToken = default)
      {
         return this.AccountsEndpoint
            .AppendPathSegments(accountId, deposits)
            .WithClient(this.client)
            .PostJsonAsync(depositFunds, cancellationToken)
            .ReceiveJson<Response<Deposit>>();
      }

      /// <inheritdoc />
      Task<Response<Deposit>> IDepositsEndpoint.CommitDepositAsync(string accountId, string depositId, CancellationToken cancellationToken = default)
      {
         return this.AccountsEndpoint
            .AppendPathSegments(accountId, deposits, depositId, "commit")
            .WithClient(this.client)
            .PostJsonAsync(null, cancellationToken)
            .ReceiveJson<Response<Deposit>>();
      }
   }
}