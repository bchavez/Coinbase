using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;
using Newtonsoft.Json;

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

      async Task<PagedResponse<PaymentMethod>> IPaymentMethodsEndpoint.ListPaymentMethodsAsync(PaginationOptions pagination, CancellationToken cancellationToken)
      {
         var responseBody = await Request(PaymentMethodsEndpoint.WithPagination(pagination))
            .GetStringAsync(cancellationToken: cancellationToken);
         if (string.IsNullOrWhiteSpace(responseBody))
            return new PagedResponse<PaymentMethod>();

         return JsonConvert.DeserializeObject<PagedResponse<PaymentMethod>>(responseBody);
      }

      async Task<Response<PaymentMethod>> IPaymentMethodsEndpoint.GetPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken)
      {
         var responseBody = await Request(PaymentMethodsEndpoint.AppendPathSegmentsRequire(paymentMethodId))
            .GetStringAsync(cancellationToken: cancellationToken);
         if (string.IsNullOrWhiteSpace(responseBody))
            return new Response<PaymentMethod>();

         return JsonConvert.DeserializeObject<Response<PaymentMethod>>(responseBody);
      }
   }
}
