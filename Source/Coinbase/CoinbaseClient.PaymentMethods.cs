using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;

namespace Coinbase
{

   public interface IPaymentMethodsEndpoint
   {
      /// <summary>
      /// Lists current user’s payment methods.
      /// </summary>
      Task<PagedResponse<PaymentMethod>> ListPaymentMethodsAsync(PaginationOptions pagination = null, CancellationToken cancellationToken = default);
      /// <summary>
      /// Show current user’s payment method.
      /// </summary>
      Task<Response<PaymentMethod>> GetPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken = default);
   }

   public partial class CoinbaseClient : IPaymentMethodsEndpoint
   {
      public IPaymentMethodsEndpoint PaymentMethods => this;

      Task<PagedResponse<PaymentMethod>> IPaymentMethodsEndpoint.ListPaymentMethodsAsync(PaginationOptions pagination, CancellationToken cancellationToken)
      {
         return Request(PaymentMethodsEndpoint.WithPagination(pagination))
            .GetJsonAsync<PagedResponse<PaymentMethod>>(cancellationToken: cancellationToken);
      }

      Task<Response<PaymentMethod>> IPaymentMethodsEndpoint.GetPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken)
      {
         return Request(PaymentMethodsEndpoint.AppendPathSegmentsRequire(paymentMethodId))
            .GetJsonAsync<Response<PaymentMethod>>(cancellationToken: cancellationToken);
      }
   }
}
