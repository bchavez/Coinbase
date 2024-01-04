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
         return Request(NotificationsEndpoint.WithPagination(pagination))
            .GetJsonAsync<PagedResponse<Notification>>(cancellationToken: cancellationToken);
      }

      Task<Response<Notification>> INotificationsEndpoint.GetNotificationAsync(string notificationId, CancellationToken cancellationToken)
      {
         return Request(NotificationsEndpoint.AppendPathSegmentsRequire(notificationId))
            .GetJsonAsync<Response<Notification>>(cancellationToken: cancellationToken);
      }
   }
}
