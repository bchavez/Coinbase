using System;
using Coinbase;
using Flurl.Http;
using static Coinbase.HeaderNames;

namespace Examples
{
   class Program
   {
      static void Main(string[] args)
      {
         Console.WriteLine("Hello World!");
         var client = new CoinbaseApi();

         client.WithHeader(TwoFactorToken, "ffff");

         client.Accounts.GetAccountAsync("fff");

         client.Users.GetAuthInfoAsync();
      }
   }
}
