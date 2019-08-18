using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;

namespace Coinbase
{

   public interface INotificationsEndpoint
   {
      /// <summary>
      /// Lists current user’s payment methods.
      /// </summary>
      Task<PagedResponse<Notification>> ListNotificationsAsync(PaginationOptions pagination = null, CancellationToken cancellationToken = default);
      /// <summary>
      /// Show current user’s payment method.
      /// </summary>
      Task<Response<Notification>> GetNotificationAsync(string notificationId, CancellationToken cancellationToken = default);
   }

   public partial class CoinbaseClient : INotificationsEndpoint
   {
      public INotificationsEndpoint Notifications => this;


      Task<PagedResponse<Notification>> INotificationsEndpoint.ListNotificationsAsync(PaginationOptions pagination, CancellationToken cancellationToken)
      {
         return this.NotificationsEndpoint
            .WithPagination(pagination)
            .WithClient(this)
            .GetJsonAsync<PagedResponse<Notification>>(cancellationToken);
      }

      Task<Response<Notification>> INotificationsEndpoint.GetNotificationAsync(string notificationId, CancellationToken cancellationToken)
      {
         return this.NotificationsEndpoint
            .AppendPathSegmentsRequire(notificationId)
            .WithClient(this)
            .GetJsonAsync<Response<Notification>>(cancellationToken);
      }

   }
}
