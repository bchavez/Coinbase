using System.Threading.Tasks;
using Coinbase.Models;
using NUnit.Framework;
using static Coinbase.Tests.Examples;

namespace Coinbase.Tests.Endpoints
{
   public class PaginationTests : OAuthServerTest
   {
      [Test]
      public async Task page_accounts()
      {
         await client.Accounts.ListAccountsAsync(new PaginationOptions {Limit = 5, EndingBefore = "before", Order = "ooo", StartingAfter = "after"});

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/accounts?limit=5&order=ooo&starting_after=after&ending_before=before");
      }

      [Test]
      public async Task page_addresses()
      {
         await client.Addresses.ListAddressesAsync("fff", new PaginationOptions { Limit = 5,Order = "ooo" });

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/accounts/fff/addresses?limit=5&order=ooo");
      }

      [Test]
      public async Task can_get_next_page()
      {
         var p = new PagedResponse<Buy>()
         {
            Pagination = new Pagination { NextUri = "/v2/next/thing?limit=5" }
         };

         await client.GetNextPageAsync(p);

         server.ShouldHaveExactCall("https://api.coinbase.com/v2/next/thing?limit=5");
      }

      //[Test]
      //public async Task can_get_prev_page()
      //{
      //   var p = new PagedResponse<Buy>()
      //   {
      //      Pagination = new Pagination { PreviousUri = "/v2/prev/thing?limit=5" }
      //   };

      //   await client.PreviousPageAsync(p);

      //   server.ShouldHaveExactCall("https://api.coinbase.com/v2/prev/thing?limit=5");
      //}
   }
}
