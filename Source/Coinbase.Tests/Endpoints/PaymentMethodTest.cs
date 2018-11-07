using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;
using static Coinbase.Tests.Examples;

namespace Coinbase.Tests.Endpoints
{
   public class PaymentMethodTests : OAuthServerTest
   {
      [Test]
      public async Task can_list()
      {
         SetupServerPagedResponse(PaginationJson, $"{PayMethod1},{PayMethod2}");

         var r = await client.PaymentMethods.ListPaymentMethodsAsync();

         var truth = new PagedResponse<PaymentMethod>
            {
               Pagination = PaginationModel,
               Data = new[]
                  {
                     PayMethod1Model, 
                     PayMethod2Model
                  }
            };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/payment-methods")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task can_get()
      {
         SetupServerSingleResponse(PayMethod2);

         var r = await client.PaymentMethods.GetPaymentMethodAsync("fff");
         
         var truth = new Response<PaymentMethod>
         {
            Data = PayMethod2Model
         };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/payment-methods/fff")
            .WithVerb(HttpMethod.Get);
      }
   }
}