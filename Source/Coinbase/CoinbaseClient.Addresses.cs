using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;
using Newtonsoft.Json;

namespace Coinbase
{
   public interface IAddressesEndpoint
   {
      /// <summary>
      /// Lists addresses for an account.
      /// </summary>
      Task<PagedResponse<AddressEntity>> ListAddressesAsync(string accountId, PaginationOptions pagination = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// Show an individual address for an account. A regular bitcoin, bitcoin cash, litecoin or ethereum address can be used in place of address_id but the address has to be associated to the correct account.
      /// </summary>
      Task<Response<AddressEntity>> GetAddressAsync(string accountId, string addressId, CancellationToken cancellationToken = default);

      /// <summary>
      /// List transactions that have been sent to a specific address. A regular bitcoin, bitcoin cash, litecoin or ethereum address can be used in place of address_id but the address has to be associated to the correct account.
      /// </summary>
      Task<PagedResponse<Transaction>> ListAddressTransactionsAsync(string accountId, string addressId, PaginationOptions pagination = null, CancellationToken cancellationToken = default);

      /// <summary>
      /// Creates a new address for an account. As all the arguments are optinal, it’s possible just to do a empty POST which will create a new address. This is handy if you need to create new receive addresses for an account on-demand.
      ///Addresses can be created for all account types.With fiat accounts, funds will be received with Instant Exchange.
      /// </summary>
      Task<Response<AddressEntity>> CreateAddressAsync(string accountId, CreateAddress createAddress, CancellationToken cancellationToken = default);
   }
   
   public partial class CoinbaseClient : IAddressesEndpoint
   {
      public IAddressesEndpoint Addresses => this;

      /// <inheritdoc />   
      async Task<PagedResponse<AddressEntity>> IAddressesEndpoint.ListAddressesAsync(string accountId, PaginationOptions pagination, CancellationToken cancellationToken)
      {
         var responseBody = await Request(
            AccountsEndpoint.AppendPathSegmentsRequire(accountId, "addresses")
                            .WithPagination(pagination)
         ).GetStringAsync(cancellationToken: cancellationToken);
         if( string.IsNullOrWhiteSpace(responseBody) ) return new PagedResponse<AddressEntity>();

         return JsonConvert.DeserializeObject<PagedResponse<AddressEntity>>(responseBody);
      }

      /// <inheritdoc />
      async Task<Response<AddressEntity>> IAddressesEndpoint.GetAddressAsync(string accountId, string addressId, CancellationToken cancellationToken)
      {
         var responseBody = await Request(
            AccountsEndpoint.AppendPathSegmentsRequire(accountId, "addresses", addressId)
            ).GetStringAsync(cancellationToken: cancellationToken);
         if( string.IsNullOrWhiteSpace(responseBody) ) return new Response<AddressEntity>();

         return JsonConvert.DeserializeObject<Response<AddressEntity>>(responseBody);
      }

      /// <inheritdoc />
      async Task<PagedResponse<Transaction>> IAddressesEndpoint.ListAddressTransactionsAsync(string accountId, string addressId, PaginationOptions pagination, CancellationToken cancellationToken)
      {
         var responseBody = await Request(
            AccountsEndpoint
               .AppendPathSegmentsRequire(accountId, "addresses", addressId, "transactions")
               .WithPagination(pagination)
         ).GetStringAsync(cancellationToken: cancellationToken);
         if( string.IsNullOrWhiteSpace(responseBody) ) return new PagedResponse<Transaction>();

         return JsonConvert.DeserializeObject<PagedResponse<Transaction>>(responseBody);
      }

      /// <inheritdoc />
      async Task<Response<AddressEntity>> IAddressesEndpoint.CreateAddressAsync(string accountId, CreateAddress createAddress, CancellationToken cancellationToken)
      {
         using var response = Request(AccountsEndpoint.AppendPathSegmentsRequire(accountId, "addresses"))
            .PostJsonAsync(createAddress, cancellationToken: cancellationToken);
         var responseBody = await response.ReceiveString();
         if( string.IsNullOrWhiteSpace(responseBody) )
            return new Response<AddressEntity>();

         return JsonConvert.DeserializeObject<Response<AddressEntity>>(responseBody);
      }
   }
}
