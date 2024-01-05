using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;
using Newtonsoft.Json;

namespace Coinbase
{
   public interface IAccountsEndpoint
   {
      /// <summary>
      /// Removes user’s account. In order to remove an account it can’t be:
      /// * Primary account
      /// * Account with non-zero balance
      /// * Fiat account
      /// * Vault with a pending withdrawal
      /// </summary>
      Task<IFlurlResponse> DeleteAccountAsync(string accountId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Show current user’s account. To access the primary account for a given currency, a currency string (BTC or ETH) can be used instead of the account id in the URL.
      /// </summary>
      Task<Response<Account>> GetAccountAsync(string accountId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Lists current user’s accounts to which the authentication method has access to.
      /// </summary>
      Task<PagedResponse<Account>> ListAccountsAsync(PaginationOptions pagination = null, CancellationToken cancellationToken = default);
      /// <summary>
      /// Promote an account as primary account.
      /// </summary>
      /// <remarks>If the operation does not work, a <c>Response&lt;Account&gt;</c>
      /// reference is returned having the account data, but it is not set to
      /// <c>primary</c>.</remarks>
      Task<Response<Account>> SetAccountAsPrimaryAsync(string accountId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Modifies user’s account.
      /// </summary>
      /// <remarks>Callers must check the result of this method to see
      /// whether the update actually occurred, as this method returns
      /// an instance of the <c>Account</c> model corresponding to the
      /// specified <paramref name="accountId"/> regardless of whether
      /// the update succeeded.</remarks>
      Task<Response<Account>> UpdateAccountAsync(string accountId, UpdateAccount updateAccount, CancellationToken cancellationToken = default);
   }


   public partial class CoinbaseClient : IAccountsEndpoint
   {
      public IAccountsEndpoint Accounts => this;

      /// <summary>
      /// Removes user’s account. In order to remove an account it can’t be:
      /// * Primary account
      /// * Account with non-zero balance
      /// * Fiat account
      /// * Vault with a pending withdrawal
      /// </summary>
      async Task<IFlurlResponse> IAccountsEndpoint.DeleteAccountAsync(string accountId, CancellationToken cancellationToken)
      {
         return await Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId))
            .DeleteAsync(cancellationToken: cancellationToken);
      }

      /// <summary>
      /// Show current user’s account. To access the primary account for a given currency, a currency string (BTC or ETH) can be used instead of the account id in the URL.
      /// </summary>
      public async Task<Response<Account>> GetAccountAsync(string accountId, CancellationToken cancellationToken)
      {
         var responseBody = await Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId)).GetStringAsync(cancellationToken: cancellationToken);
         if (string.IsNullOrWhiteSpace(responseBody))
            return new Response<Account>();

         return JsonConvert.DeserializeObject<Response<Account>>(responseBody);
      }

      /// <summary>
      /// Lists current user’s accounts to which the authentication method has access to.
      /// </summary>
      async Task<PagedResponse<Account>> IAccountsEndpoint.ListAccountsAsync(PaginationOptions pagination, CancellationToken cancellationToken)
      {
         var responseBody = await Request(AccountsEndpoint.WithPagination(pagination))
            .GetStringAsync(cancellationToken: cancellationToken);
         if( string.IsNullOrWhiteSpace(responseBody) ) return default;
            
         return JsonConvert.DeserializeObject<PagedResponse<Account>>(responseBody);
      }
      /// <summary>
      /// Promote an account as primary account.
      /// </summary>
      /// <remarks>If the operation does not work, a <c>Response&lt;Account&gt;</c>
      /// reference is returned having the account data, but it is not set to
      /// <c>primary</c>.</remarks>
      async Task<Response<Account>> IAccountsEndpoint.SetAccountAsPrimaryAsync(string accountId, CancellationToken cancellationToken)
      {
         using var response = Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, "primary"))
            .PostJsonAsync(null, cancellationToken: cancellationToken);
         var responseBody = await response.ReceiveString();
         if( string.IsNullOrWhiteSpace(responseBody) )
            return await GetAccountAsync(accountId, cancellationToken);

         return JsonConvert.DeserializeObject<Response<Account>>(responseBody);
      }
      /// <summary>
      /// Modifies user’s account.
      /// </summary>
      /// <remarks>Callers must check the result of this method to see
      /// whether the update actually occurred, as this method returns
      /// an instance of the <c>Account</c> model corresponding to the
      /// specified <paramref name="accountId"/> regardless of whether
      /// the update succeeded.</remarks>
      async Task<Response<Account>> IAccountsEndpoint.UpdateAccountAsync(string accountId, UpdateAccount updateAccount, CancellationToken cancellationToken)
      {
         using var response = Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId))
            .PutJsonAsync(updateAccount, cancellationToken: cancellationToken);
         var responseBody = await response.ReceiveString();
         if( string.IsNullOrWhiteSpace(responseBody) )
            return await GetAccountAsync(accountId, cancellationToken);

         return JsonConvert.DeserializeObject<Response<Account>>(responseBody);
      }
   }
}
