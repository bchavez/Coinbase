using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;
using static Coinbase.Tests.Examples;

namespace Coinbase.Tests.Endpoints
{
   public class WithdrawlTests : OAuthServerTest
   {
      [Test]
      public async Task can_list()
      {
         SetupServerPagedResponse(PaginationJson, $"{Withdrawal1}");

         var r = await client.Withdrawals.ListWithdrawalsAsync("fff");

         var truth = new PagedResponse<Withdrawal>
            {
               Pagination = PaginationModel,
               Data = new[]
                  {
                     Withdrawal1Model
                  }
            };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/accounts/fff/withdrawals")
            .WithVerb(HttpMethod.Get);
      }

      [Test]
      public async Task can_get()
      {
         SetupServerSingleResponse(Withdrawal1);

         var r = await client.Withdrawals.GetWithdrawalAsync("fff", "uuu");

         var truth = new Response<Withdrawal>
         {
            Data = Withdrawal1Model
         };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/accounts/fff/withdrawals/uuu")
            .WithVerb(HttpMethod.Get);
      }



      [Test]
      public async Task can_withdrawal()
      {
         SetupServerSingleResponse(Withdrawal1);

         var create = new WithdrawalFunds
            {
               Amount = 10.0m,
               Currency = "USD",
               PaymentMethod = "B28EB04F-BD70-4308-90A1-96065283A001"
         };
         var r = await client.Withdrawals.WithdrawalFundsAsync("fff", create );

         var truth = new Response<Withdrawal>
         {
            Data = Withdrawal1Model
         };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveRequestBody(
            @"{""amount"":10.0,""currency"":""USD"",""payment_method"":""B28EB04F-BD70-4308-90A1-96065283A001"",""commit"":false}");

         server.ShouldHaveExactCall($"https://api.coinbase.com/v2/accounts/fff/withdrawals")
            .WithVerb(HttpMethod.Post);
      }


      [Test]
      public async Task can_commit()
      {
         SetupServerSingleResponse(Withdrawal1);

         var r = await client.Withdrawals.CommitWithdrawalAsync("fff", "uuu");

         var truth = new Response<Withdrawal>
            {
               Data = Withdrawal1Model
            };

         truth.Should().BeEquivalentTo(r);

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/accounts/fff/withdrawals/uuu/commit")
            .WithVerb(HttpMethod.Post);
      }
   }
}