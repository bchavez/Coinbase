using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;

namespace Coinbase
{
   public interface IAccountsEndpoint
   {
      /// <summary>
      /// Lists current user’s accounts to which the authentication method has access to.
      /// </summary>
      Task<PagedResponse<Account>> ListAccountsAsync(PaginationOptions pagination = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// Show current user’s account. To access the primary account for a given currency, a currency string (BTC or ETH) can be used instead of the account id in the URL.
      /// </summary>
      Task<Response<Account>> GetAccountAsync(string accountId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Promote an account as primary account.
      /// </summary>
      Task<Response<Account>> SetAccountAsPrimaryAsync(string accountId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Modifies user’s account.
      /// </summary>
      Task<Response<Account>> UpdateAccountAsync(string accountId, UpdateAccount updateAccount, CancellationToken cancellationToken = default);

      /// <summary>
      /// Removes user’s account. In order to remove an account it can’t be:
      /// * Primary account
      /// * Account with non-zero balance
      /// * Fiat account
      /// * Vault with a pending withdrawal
      /// </summary>
      Task<IFlurlResponse> DeleteAccountAsync(string accountId, CancellationToken cancellationToken = default);
   }


   public partial class CoinbaseClient : IAccountsEndpoint
   {
      public IAccountsEndpoint Accounts => this;

      /// <summary>
      /// Lists current user’s accounts to which the authentication method has access to.
      /// </summary>
      Task<PagedResponse<Account>> IAccountsEndpoint.ListAccountsAsync(PaginationOptions pagination, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .WithPagination(pagination)
            .WithClient(this)
            .GetJsonAsync<PagedResponse<Account>>(cancellationToken);
      }

      /// <summary>
      /// Show current user’s account. To access the primary account for a given currency, a currency string (BTC or ETH) can be used instead of the account id in the URL.
      /// </summary>
      Task<Response<Account>> IAccountsEndpoint.GetAccountAsync(string accountId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId)
            .WithClient(this)
            .GetJsonAsync<Response<Account>>(cancellationToken);
      }

      /// <summary>
      /// Promote an account as primary account.
      /// </summary>
      Task<Response<Account>> IAccountsEndpoint.SetAccountAsPrimaryAsync(string accountId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "primary")
            .WithClient(this)
            .PostJsonAsync(null, cancellationToken)
            .ReceiveJson<Response<Account>>();
      }
      /// <summary>
      /// Modifies user’s account.
      /// </summary>
      Task<Response<Account>> IAccountsEndpoint.UpdateAccountAsync(string accountId, UpdateAccount updateAccount, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId)
            .WithClient(this)
            .PutJsonAsync(updateAccount, cancellationToken)
            .ReceiveJson<Response<Account>>();
      }

      /// <summary>
      /// Removes user’s account. In order to remove an account it can’t be:
      /// * Primary account
      /// * Account with non-zero balance
      /// * Fiat account
      /// * Vault with a pending withdrawal
      /// </summary>
      Task<IFlurlResponse> IAccountsEndpoint.DeleteAccountAsync(string accountId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId)
            .WithClient(this)
            .DeleteAsync(cancellationToken);
      }
   }
}
