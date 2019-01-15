using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;
using static Coinbase.Tests.Examples;

namespace Coinbase.Tests.Endpoints
{
   public class SellTests : OAuthServerTest
   {
      [Test]
      public async Task can_list()
      {
         SetupServerPagedResponse(PaginationJson, $"{Sell1}");

         var r = await client.Sells.ListSellsAsync("fff");

         var truth = new PagedResponse<Sell>
            {
               Pagination = PaginationModel,
               Data = new[]
                  {
                     Sell1Model
                  }
            };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/accounts/fff/sells")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task can_get()
      {
         SetupServerSingleResponse(Sell1);

         var r = await client.Sells.GetSellAsync("fff", "uuu");

         var truth = new Response<Sell>
         {
            Data = Sell1Model
         };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/accounts/fff/sells/uuu")
            .WithVerb(HttpMethod.Get);
      }



      [Test]
      public async Task can_place_sellorder()
      {
         SetupServerSingleResponse(Sell2);

         var create = new PlaceSell
            {
               Amount = 10m,
               Currency = "BTC",
               PaymentMethod = "B28EB04F-BD70-4308-90A1-96065283A001"
         };
         var r = await client.Sells.PlaceSellOrderAsync("fff", create );

         var truth = new Response<Sell>
         {
            Data = Sell2Model
         };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveRequestBody(
            @"{""amount"":10.0,""currency"":""BTC"",""payment_method"":""B28EB04F-BD70-4308-90A1-96065283A001"",""agree_btc_amount_varies"":false,""commit"":false,""quote"":false}");

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/accounts/fff/sells")
            .WithVerb(HttpMethod.Post);
      }


      [Test]
      public async Task can_commit()
      {
         SetupServerSingleResponse(Sell2);

         var r = await client.Sells.CommitSellAsync("fff", "uuu");

         var truth = new Response<Sell>
            {
               Data = Sell2Model
            };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/accounts/fff/sells/uuu/commit")
            .WithVerb(HttpMethod.Post);
      }
   }
}
