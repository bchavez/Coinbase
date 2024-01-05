using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;
using Newtonsoft.Json;

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
         return Request(
               AccountsEndpoint.AppendPathSegmentsRequire(accountId, deposits)
                               .WithPagination(pagination)
            )
            .GetJsonAsync<PagedResponse<Deposit>>(cancellationToken: cancellationToken);
      }

      /// <inheritdoc />
      Task<Response<Deposit>> IDepositsEndpoint.GetDepositAsync(string accountId, string depositId, CancellationToken cancellationToken)
      {
         return Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, deposits, depositId))
            .GetJsonAsync<Response<Deposit>>(cancellationToken: cancellationToken);
      }

      /// <inheritdoc />
      async Task<Response<Deposit>> IDepositsEndpoint.DepositFundsAsync(string accountId, DepositFunds depositFunds, CancellationToken cancellationToken)
      {
         using var response = Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, deposits))
            .PostJsonAsync(depositFunds, cancellationToken: cancellationToken);
         var responseBody = await response.ReceiveString();
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new Response<Deposit>();

         return JsonConvert.DeserializeObject<Response<Deposit>>(responseBody);
      }

      /// <inheritdoc />
      async Task<Response<Deposit>> IDepositsEndpoint.CommitDepositAsync(string accountId, string depositId, CancellationToken cancellationToken)
      {
         using var response = Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, deposits, depositId, "commit"))
            .PostJsonAsync(null, cancellationToken: cancellationToken);
         var responseBody = await response.ReceiveString();
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new Response<Deposit>();

         return JsonConvert.DeserializeObject<Response<Deposit>>(responseBody);
      }
   }
}
