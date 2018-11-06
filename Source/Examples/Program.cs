using System;
using Coinbase;
using Flurl.Http;

namespace Examples
{
   class Program
   {
      static void Main(string[] args)
      {
         Console.WriteLine("Hello World!");
         var api = new CoinbaseApi();

         var c = api.GetCurrentClient();
         c.WithHeader(HeaderNames.TwoFactorToken, "ffff");

         api.Accounts.GetAccountAsync("fff");
      }
   }
}
