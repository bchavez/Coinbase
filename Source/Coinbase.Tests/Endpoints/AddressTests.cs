using System.Net.Http;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using NUnit.Framework;
using System;
using static Coinbase.Tests.Examples;

namespace Coinbase.Tests.Endpoints
{
   [TestFixture]
   public class AddressTests : OAuthServerTest
   {
      [Test]
      public async Task can_list_addresses()
      {
         SetupServerPagedResponse(PaginationJson, $"{Address1},{Address2}");

         var accounts = await client.Addresses.ListAddressesAsync("ffff");

         var truth = new PagedResponse<AddressEntity>
         {
            Pagination = PaginationModel,
            Data = new[] { Address1Model, Address2Model }
         };

         truth.Should().BeEquivalentTo(accounts);

         server.ShouldHaveCalled("https://api.coinbase.com/v2/accounts/ffff/addresses")
            .WithVerb(HttpMethod.Get);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }

      [Test]
      public async Task get_an_address()
      {
         SetupServerSingleResponse(Address1);

         var account = await client.Addresses.GetAddressAsync("ffff", Address1Model.Id);

         var truth = new Response<AddressEntity>
         {
            Data = Address1Model
         };

         truth.Should().BeEquivalentTo(account);

         server.ShouldHaveCalled($"https://api.coinbase.com/v2/accounts/ffff/addresses/{Address1Model.Id}")
            .WithVerb(HttpMethod.Get);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }

      [Test]
      public async Task can_list_address_transactions()
      {
         SetupServerPagedResponse(PaginationJson, $"{Transaction1}");

         var txs = await client.Addresses.ListAddressTransactionsAsync("fff", "uuu");

         var truth = new PagedResponse<Transaction>
         {
            Pagination = PaginationModel,
            Data = new []{Transaction1Model}
         };

         truth.Should().BeEquivalentTo(txs);

         server.ShouldHaveCalled($"https://api.coinbase.com/v2/accounts/fff/addresses/uuu/transactions")
            .WithVerb(HttpMethod.Get);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }

      [Test]
      public async Task can_create_address()
      {
         SetupServerSingleResponse(Address1WithName("ddd"));

         var create = new CreateAddress { Name = "ddd"};
         var add = await client.Addresses.CreateAddressAsync("fff", create);

         var truth = new Response<AddressEntity>
            {
               Data = Address1Model
            };

         truth.Data.Name = "ddd";

         truth.Should().BeEquivalentTo(add);

         server.ShouldHaveCalled("https://api.coinbase.com/v2/accounts/fff/addresses")
            .WithVerb(HttpMethod.Post);

         Console.WriteLine("*** UNIT TEST PASSED ***");
      }
   }
}
