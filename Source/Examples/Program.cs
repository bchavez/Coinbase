using System;
using System.Threading.Tasks;
using Coinbase;
using Coinbase.Models;
using Flurl.Http;
using static Coinbase.HeaderNames;

namespace Examples
{
   class Program
   {
      static async Task Main(string[] args)
      {
         Console.WriteLine("Hello World!");
         var client = new PublicCoinbaseApi();

         var create = new CreateTransaction
            {
               Amount = 1.0m,
               Currency = "BTC"
            };
         var response = await client
            .WithHeader(TwoFactorToken, "ffff")
            .AllowAnyHttpStatus()
            .Transactions.SendMoneyAsync("accountId", create);

         if( response.HasError() )
         {
            // transaction is okay!
         }
      }
   }
}
