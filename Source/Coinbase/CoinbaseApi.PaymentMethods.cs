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
      Task<PagedResponse<PaymentMethod>> ListPaymentMethods(CancellationToken cancellationToken = default);
      /// <summary>
      /// Show current user’s payment method.
      /// </summary>
      Task<Response<PaymentMethod>> GetPaymentMethod(string paymentMethodId, CancellationToken cancellationToken = default);
   }

   public partial class CoinbaseApi : IPaymentMethodsEndpoint
   {
      public IPaymentMethodsEndpoint PaymentMethods => this;


      public Task<PagedResponse<PaymentMethod>> ListPaymentMethods(CancellationToken cancellationToken = default)
      {
         return this.PaymentMethodsEndpoint
            .WithClient(this.client)
            .GetJsonAsync<PagedResponse<PaymentMethod>>(cancellationToken);
      }

      public Task<Response<PaymentMethod>> GetPaymentMethod(string paymentMethodId, CancellationToken cancellationToken = default)
      {
         return this.PaymentMethodsEndpoint
            .AppendPathSegment(paymentMethodId)
            .WithClient(this.client)
            .GetJsonAsync<Response<PaymentMethod>>(cancellationToken);
      }

   }
}