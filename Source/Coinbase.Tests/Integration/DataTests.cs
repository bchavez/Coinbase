using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Flurl.Http.Configuration;
using NUnit.Framework;
using Z.ExtensionMethods;

namespace Coinbase.Tests.Integration
{

   [Explicit]
   public class IntegrationTests
   {
      protected CoinbaseApi api;

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

         api = new CoinbaseApi(new Config{ ApiKey = apiKey, ApiSecret = apiSecret});
      }
   }

   [Explicit]
   public class UserTests : IntegrationTests
   {
      [Test]
      public async Task can_get_auths()
      {
         var r = await api.Users.GetAuthInfoAsync();
         r.Dump();
      }
   }

   public class DataTests 
   {
      private CoinbaseApi api;

      [SetUp]
      public void BeforeEachTest()
      {
         api = new CoinbaseApi();
      }

      [Test]
      public async Task can_get_currencies()
      {
         var r = await api.Data.GetCurrenciesAsync();
         var usd = r.Data.Where(c => c.Id == "USD").First();
         usd.Name.Should().StartWith("United States");
      }

      [Test]
      public async Task can_get_exchange_rates()
      {
         var r = await api.Data.GetExchangeRatesAsync("ETH");
         r.Data.Currency.Should().Be("ETH");
         r.Data.Rates["USD"].Should().BeGreaterThan(5);
      }

      [Test]
      public async Task can_get_buyprice()
      {
         var r = await api.Data.GetBuyPriceAsync("ETH-USD");
         r.Dump();
         r.Data.Amount.Should().BeGreaterThan(5);
         r.Data.Currency.Should().Be("USD");
         r.Data.Base.Should().Be("ETH");
      }
      [Test]
      public async Task can_get_sellprice()
      {
         var r = await api.Data.GetSellPriceAsync("ETH-USD");
         r.Dump();
         r.Data.Amount.Should().BeGreaterThan(5);
         r.Data.Currency.Should().Be("USD");
         r.Data.Base.Should().Be("ETH");
      }
      [Test]
      public async Task can_get_spotprice()
      {
         var r = await api.Data.GetSpotPriceAsync("ETH-USD");
         r.Dump();
         r.Data.Amount.Should().BeGreaterThan(5);
         r.Data.Currency.Should().Be("USD");
         r.Data.Base.Should().Be("ETH");
      }

      [Test]
      public async Task can_get_time()
      {
         var r = await api.Data.GetCurrentTimeAsync();
         r.Dump();
         r.Data.Iso.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromHours(1));
      }
   }

   public class ProxyFactory : DefaultHttpClientFactory
   {
      private readonly WebProxy proxy;

      public ProxyFactory(WebProxy proxy)
      {
         this.proxy = proxy;
      }

      public override HttpMessageHandler CreateMessageHandler()
      {
         return new HttpClientHandler
            {
               Proxy = this.proxy,
               UseProxy = true
            };
      }
   }
}