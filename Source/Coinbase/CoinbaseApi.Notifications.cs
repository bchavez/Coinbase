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
      Task<PagedResponse<Notification>> ListNotificationsAsync(CancellationToken cancellationToken = default);
      /// <summary>
      /// Show current user’s payment method.
      /// </summary>
      Task<Response<Notification>> GetNotificationAsync(string notificationId, CancellationToken cancellationToken = default);
   }

   public partial class CoinbaseApi : INotificationsEndpoint
   {
      public INotificationsEndpoint Notifications => this;


      Task<PagedResponse<Notification>> INotificationsEndpoint.ListNotificationsAsync(CancellationToken cancellationToken)
      {
         return this.NotificationsEndpoint
            .WithClient(this.client)
            .GetJsonAsync<PagedResponse<Notification>>(cancellationToken);
      }

      Task<Response<Notification>> INotificationsEndpoint.GetNotificationAsync(string notificationId, CancellationToken cancellationToken)
      {
         return this.NotificationsEndpoint
            .AppendPathSegment(notificationId)
            .WithClient(this.client)
            .GetJsonAsync<Response<Notification>>(cancellationToken);
      }

   }
}