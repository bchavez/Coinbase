using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;
using static Coinbase.Tests.Examples;

namespace Coinbase.Tests.Endpoints
{
   public class NotificationTests : OAuthServerTest
   {
      [Test]
      public async Task can_list()
      {
         SetupServerPagedResponse(PaginationJson, $"{Notification1}");

         var r = await client.Notifications.ListNotificationsAsync();

         var truth = new PagedResponse<Notification>
            {
               Pagination = PaginationModel,
               Data = new[]
                  {
                     Notification1Model
                  }
            };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/notifications")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task can_get()
      {
         SetupServerSingleResponse(Notification2);

         var r = await client.Notifications.GetNotificationAsync("fff");
         
         var truth = new Response<Notification>
         {
            Data = Notification2Model
         };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/notifications/fff")
            .WithVerb(HttpMethod.Get);
      }
   }
}