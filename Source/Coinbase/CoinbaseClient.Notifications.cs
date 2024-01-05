using System;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using Flurl.Http;
using Newtonsoft.Json;

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

      async Task<PagedResponse<Notification>> INotificationsEndpoint.ListNotificationsAsync(PaginationOptions pagination, CancellationToken cancellationToken)
      {
         var responseBody = await Request(NotificationsEndpoint.WithPagination(pagination))
                            .GetStringAsync(cancellationToken: cancellationToken);
         if (string.IsNullOrWhiteSpace(responseBody))
            return new PagedResponse<Notification>();

         return JsonConvert.DeserializeObject<PagedResponse<Notification>>(responseBody);
      }

      async Task<Response<Notification>> INotificationsEndpoint.GetNotificationAsync(string notificationId, CancellationToken cancellationToken)
      {
         var responseBody = await Request(NotificationsEndpoint.AppendPathSegmentsRequire(notificationId))
            .GetStringAsync(cancellationToken: cancellationToken);
         if( string.IsNullOrWhiteSpace(responseBody) ) return new Response<Notification>();

         return JsonConvert.DeserializeObject<Response<Notification>>(responseBody);
      }
   }
}
