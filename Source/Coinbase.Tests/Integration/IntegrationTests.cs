using System.IO;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using NUnit.Framework;
using Z.ExtensionMethods;

namespace Coinbase.Tests.Integration
{
   [Explicit]
   public class IntegrationTests
   {
      protected CoinbaseApi client;

      public IntegrationTests()
      {
         Directory.SetCurrentDirectory(Path.GetDirectoryName(typeof(IntegrationTests).Assembly.Location));
         var lines = File.ReadAllLines("../../.secrets.txt");
         var apiKey = lines[0].GetAfter(":");
         var apiSecret = lines[1].GetAfter(":");

         var webProxy = new WebProxy("http://localhost.:8888", BypassOnLocal: false);

         FlurlHttp.Configure(settings =>
            {
               settings.HttpClientFactory = new ProxyFactory(webProxy);
            });

         client = new CoinbaseApi(new ApiKeyConfig{ ApiKey = apiKey, ApiSecret = apiSecret});
      }
   }
   [Explicit]
   public class UserTests : IntegrationTests
   {
      [Test]
      public async Task can_get_auths()
      {
         var r = await client.Users.GetAuthInfoAsync();
         r.Dump();
      }

      [Test]
      public async Task check_account_list()
      {
         var r = await client.Accounts.ListAccountsAsync();
         r.Dump();
      }

      [Test]
      public async Task check_account_transactions()
      {
         var r = await client.Transactions.ListTransactionsAsync("fff");

         r.Dump();
      }

      [Test]
      public async Task check_invalid_account()
      {
         var r = await client.Accounts.GetAccountAsync("fff");
         r.Dump();
      }
   }
}