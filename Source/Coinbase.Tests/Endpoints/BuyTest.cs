using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;
using static Coinbase.Tests.Examples;

namespace Coinbase.Tests.Endpoints
{
   public class BuyTests : OAuthServerTest
   {
      [Test]
      public async Task can_list()
      {
         SetupServerPagedResponse(PaginationJson, $"{Buy1}");

         var r = await client.Buys.ListBuysAsync("fff");

         var truth = new PagedResponse<Buy>
            {
               Pagination = PaginationModel,
               Data = new[]
                  {
                     Buy1Model
                  }
            };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/accounts/fff/buys")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task can_get()
      {
         SetupServerSingleResponse(Buy1);

         var r = await client.Buys.GetBuyAsync("fff", "uuu");

         var truth = new Response<Buy>
         {
            Data = Buy1Model
         };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/accounts/fff/buys/uuu")
            .WithVerb(HttpMethod.Get);
      }



      [Test]
      public async Task can_place_buyorder()
      {
         SetupServerSingleResponse(Buy2);

         var create = new PlaceBuy
            {
               Amount = 0.1m,
               Currency = "BTC",
               PaymentMethod = "B28EB04F-BD70-4308-90A1-96065283A001"
         };
         var r = await client.Buys.PlaceBuyOrderAsync("fff", create );

         var truth = new Response<Buy>
         {
            Data = Buy2Model
         };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveRequestBody(
            @"{""amount"":0.1,""currency"":""BTC"",""payment_method"":""B28EB04F-BD70-4308-90A1-96065283A001"",""agree_btc_amount_varies"":false,""commit"":false,""quote"":false}");

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/accounts/fff/buys")
            .WithVerb(HttpMethod.Post);
      }


      [Test]
      public async Task can_commit()
      {
         SetupServerSingleResponse(Buy2);

         var r = await client.Buys.CommitBuyAsync("fff", "uuu");

         var truth = new Response<Buy>
            {
               Data = Buy2Model
            };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/accounts/fff/buys/uuu/commit")
            .WithVerb(HttpMethod.Post);
      }
   }
}