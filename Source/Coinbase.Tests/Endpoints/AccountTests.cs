using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Configuration;
using static Coinbase.Tests.Examples;

namespace Coinbase.Tests.Endpoints
{
   [TestFixture]
   public class AccountTests : OAuthServerTest
   {
      [Test]
      public void can_list_accounts()
      {
         SetupServerPagedResponse(PaginationJson, $"{Account1},{Account2}");

         PagedResponse<Account> accounts = default;

         Assert.DoesNotThrowAsync(async () =>
            accounts = await client.Accounts.ListAccountsAsync(
               new PaginationOptions
               {
                  Limit = 25, Order = "desc"
               })
            );

         Assert.That(accounts, Is.Not.Null);

         Assert.That(accounts.Data.Length == 2);

         var truth = new PagedResponse<Account>
            {
               Pagination = PaginationModel,
               Data = new[] {Account1Model, Account2Model}
            };

         truth.Should().BeEquivalentTo(accounts);

         server.ShouldHaveCalled("https://api.coinbase.com/v2/accounts")
            .WithVerb(HttpMethod.Get);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }
         
      [Test]
      public async Task get_an_account()
      {
         SetupServerSingleResponse(Account2);

         var account = await client.Accounts.GetAccountAsync(Account2Model.Id);

         var truth = new Response<Account>
            {
               Data = Account2Model
            };

         truth.Should().BeEquivalentTo(account);

         server.ShouldHaveCalled($"https://api.coinbase.com/v2/accounts/{Account2Model.Id}")
            .WithVerb(HttpMethod.Get);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }

      [Test]
      public async Task can_set_account_as_primary()
      {
         SetupServerSingleResponse(Account3);

         var account = await client.Accounts.SetAccountAsPrimaryAsync(Account3Model.Id);

         var truth = new Response<Account>
            {
               Data = Account3Model
            };

         truth.Should().BeEquivalentTo(account);

         server.ShouldHaveCalled($"https://api.coinbase.com/v2/accounts/{Account3Model.Id}/primary")
            .WithVerb(HttpMethod.Post);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }

      [Test]
      public async Task can_update_account()
      {
         SetupServerSingleResponse(Account3WithNameChange("New account name"));

         var update = new UpdateAccount {Name = "New account name"};
         var account = await client.Accounts.UpdateAccountAsync(Account3Model.Id, update);

         var truth = new Response<Account>
            {
               Data = Account3Model
            };
         truth.Data.Name = "New account name";

         truth.Should().BeEquivalentTo(account);

         server.ShouldHaveCalled($"https://api.coinbase.com/v2/accounts/{Account3Model.Id}")
            .WithVerb(HttpMethod.Put);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }

      [Test]
      public async Task can_delete_account()
      {
         server.RespondWith(status: 204);
         var r = await client.Accounts.DeleteAccountAsync("ffff");
         
         r.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

         server.ShouldHaveCalled("https://api.coinbase.com/v2/accounts/ffff")
            .WithVerb(HttpMethod.Delete);
      }
   }
}
