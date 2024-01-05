using JsonSerializer = System.Text.Json.JsonSerializer;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static Coinbase.Tests.Examples;

namespace Coinbase.Tests.Endpoints
{
   [TestFixture]
   public class DepositTests : OAuthServerTest
   {
      [Test]
      public async Task can_commit()
      {
         SetupServerSingleResponse(Deposit1);

         var r = await client.Deposits.CommitDepositAsync("fff", "uuu");

         var truth = new Response<Deposit> { Data = Deposit1Model };

         truth.Should()
              .BeEquivalentTo(r);

         server.ShouldHaveCalled("https://api.coinbase.com/v2/accounts/fff/deposits/uuu/commit")
               .WithVerb(HttpMethod.Post);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }

      [Test]
      public async Task can_depositfunds()
      {
         SetupServerSingleResponse(Deposit1);

         var create = new DepositFunds { Amount = 10.0m, Currency = "USD", PaymentMethod = "B28EB04F-BD70-4308-90A1-96065283A001" };
         var r = await client.Deposits.DepositFundsAsync("fff", create);

         var truth = new Response<Deposit> { Data = Deposit1Model };

         truth.Should()
              .BeEquivalentTo(r);

         server.ShouldHaveCalled("https://api.coinbase.com/v2/accounts/fff/deposits")
               .WithRequestBody(JsonSerializer.Serialize(create))
               .WithVerb(HttpMethod.Post);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }

      [Test]
      public async Task can_get()
      {
         SetupServerSingleResponse(Deposit1);

         var r = await client.Deposits.GetDepositAsync("fff", "uuu");

         var truth = new Response<Deposit> { Data = Deposit1Model };

         truth.Should()
              .BeEquivalentTo(r);

         server.ShouldHaveCalled("https://api.coinbase.com/v2/accounts/fff/deposits/uuu")
               .WithVerb(HttpMethod.Get);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }

      [Test]
      public async Task can_list()
      {
         SetupServerPagedResponse(PaginationJson, $"{Deposit1}");

         var r = await client.Deposits.ListDepositsAsync("fff");

         var truth = new PagedResponse<Deposit> { Pagination = PaginationModel, Data = new[] { Deposit1Model } };

         truth.Should()
              .BeEquivalentTo(r);

         server.ShouldHaveCalled("https://api.coinbase.com/v2/accounts/fff/deposits")
               .WithVerb(HttpMethod.Get);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }
   }
}
