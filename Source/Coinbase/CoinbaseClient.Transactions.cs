using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;

namespace Coinbase
{
   public interface ITransactionsEndpoint
   {
      /// <summary>
      /// Lists account’s transactions.
      /// </summary>
      Task<PagedResponse<Transaction>> ListTransactionsAsync(string accountId, PaginationOptions pagination = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// Show an individual transaction for an account. See transaction resource for more information.
      /// </summary>
      Task<Response<Transaction>> GetTransactionAsync(string accountId, string transactionId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Send funds to a bitcoin address, bitcoin cash address, litecoin address, ethereum address, or email address. No transaction fees are required for off blockchain bitcoin transactions.
      /// It’s recommended to always supply a unique idem field for each transaction.This prevents you from sending the same transaction twice if there has been an unexpected network outage or other issue.
      /// When used with OAuth2 authentication, this endpoint requires two factor authentication unless used with wallet:transactions:send:bypass-2fa scope.
      ///If the user is able to buy bitcoin, they can send funds from their fiat account using instant exchange feature.Buy fees will be included in the created transaction and the recipient will receive the user defined amount.
      /// </summary>
      Task<Response<Transaction>> SendMoneyAsync(string accountId, CreateTransaction createTransaction, CancellationToken cancellationToken = default);

      /// <summary>
      /// Transfer bitcoin, bitcoin cash, litecoin or ethereum between two of a user’s accounts. Following transfers are allowed:
      /// * wallet to wallet
      /// * wallet to vault
      /// </summary>
      Task<Response<Transaction>> TransferMoneyAsync(string accountId, CreateTransfer createTransfer, CancellationToken cancellationToken = default);

      /// <summary>
      /// Requests money from an email address.
      /// </summary>
      Task<Response<Transaction>> RequestMoneyAsync(string accountId, RequestMoney requestMoney, CancellationToken cancellationToken = default);

      /// <summary>
      /// Lets the recipient of a money request complete the request by sending money to the user who requested the money. This can only be completed by the user to whom the request was made, not the user who sent the request.
      /// </summary>
      Task<IFlurlResponse> CompleteRequestMoneyAsync(string accountId, string transactionId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Lets the user resend a money request. This will notify recipient with a new email.
      /// </summary>
      Task<IFlurlResponse> ResendRequestMoneyAsync(string accountId, string transactionId, CancellationToken cancellationToken = default);

      /// <summary>
      /// Lets a user cancel a money request. Money requests can be canceled by the sender or the recipient.
      /// </summary>
      Task<IFlurlResponse> CancelRequestMoneyAsync(string accountId, string transactionId, CancellationToken cancellationToken = default);
   }


   public partial class CoinbaseClient : ITransactionsEndpoint
   {
      public ITransactionsEndpoint Transactions => this;

      /// <summary>
      /// Lists account’s transactions.
      /// </summary>
      Task<PagedResponse<Transaction>> ITransactionsEndpoint.ListTransactionsAsync(string accountId, PaginationOptions pagination, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "transactions")
            .WithPagination(pagination)
            .WithClient(this)
            .GetJsonAsync<PagedResponse<Transaction>>(cancellationToken);
      }

      /// <summary>
      /// Show an individual transaction for an account. See transaction resource for more information.
      /// </summary>
      Task<Response<Transaction>> ITransactionsEndpoint.GetTransactionAsync(string accountId, string transactionId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "transactions", transactionId)
            .WithClient(this)
            .GetJsonAsync<Response<Transaction>>(cancellationToken);
      }

      /// <summary>
      /// Send funds to a bitcoin address, bitcoin cash address, litecoin address, ethereum address, or email address. No transaction fees are required for off blockchain bitcoin transactions.
      /// It’s recommended to always supply a unique idem field for each transaction.This prevents you from sending the same transaction twice if there has been an unexpected network outage or other issue.
      /// When used with OAuth2 authentication, this endpoint requires two factor authentication unless used with wallet:transactions:send:bypass-2fa scope.
      ///If the user is able to buy bitcoin, they can send funds from their fiat account using instant exchange feature.Buy fees will be included in the created transaction and the recipient will receive the user defined amount.
      /// </summary>
      Task<Response<Transaction>> ITransactionsEndpoint.SendMoneyAsync(string accountId, CreateTransaction createTransaction, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "transactions")
            .WithClient(this)
            .PostJsonAsync(createTransaction, cancellationToken)
            .ReceiveJson<Response<Transaction>>();

      }

      /// <summary>
      /// Transfer bitcoin, bitcoin cash, litecoin or ethereum between two of a user’s accounts. Following transfers are allowed:
      /// * wallet to wallet
      /// * wallet to vault
      /// </summary>
      Task<Response<Transaction>> ITransactionsEndpoint.TransferMoneyAsync(string accountId, CreateTransfer createTransfer, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "transactions")
            .WithClient(this)
            .PostJsonAsync(createTransfer, cancellationToken)
            .ReceiveJson<Response<Transaction>>();
      }

      /// <summary>
      /// Requests money from an email address.
      /// </summary>
      Task<Response<Transaction>> ITransactionsEndpoint.RequestMoneyAsync(string accountId, RequestMoney requestMoney, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "transactions")
            .WithClient(this)
            .PostJsonAsync(requestMoney, cancellationToken)
            .ReceiveJson<Response<Transaction>>();
      }

      /// <summary>
      /// Lets the recipient of a money request complete the request by sending money to the user who requested the money. This can only be completed by the user to whom the request was made, not the user who sent the request.
      /// </summary>
      Task<IFlurlResponse> ITransactionsEndpoint.CompleteRequestMoneyAsync(string accountId, string transactionId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "transactions", transactionId, "complete")
            .WithClient(this)
            .PostJsonAsync(null, cancellationToken);
      }


      /// <summary>
      /// Lets the user resend a money request. This will notify recipient with a new email.
      /// </summary>
      Task<IFlurlResponse> ITransactionsEndpoint.ResendRequestMoneyAsync(string accountId, string transactionId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "transactions", transactionId, "resend")
            .WithClient(this)
            .PostJsonAsync(null, cancellationToken);
      }

      /// <summary>
      /// Lets a user cancel a money request. Money requests can be canceled by the sender or the recipient.
      /// </summary>
      Task<IFlurlResponse> ITransactionsEndpoint.CancelRequestMoneyAsync(string accountId, string transactionId, CancellationToken cancellationToken)
      {
         return this.AccountsEndpoint
            .AppendPathSegmentsRequire(accountId, "transactions", transactionId)
            .WithClient(this)
            .DeleteAsync(cancellationToken);
      }

   }
}
