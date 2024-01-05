﻿using System;
using System.Diagnostics;
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
   public class DataTests 
   {
      private CoinbaseClient client;

      [OneTimeSetUp]
      public void BeforeAllTests()
      {
         if( !Environment.OSVersion.IsAppVeyor() && Process.GetProcessesByName("Fiddler").Any() )
         {
            FlurlHttp.Clients.WithDefaults(builder => builder.ConfigureInnerHandler(
               hch =>
                  {
                     hch.Proxy = new WebProxy("http://localhost.:8888", BypassOnLocal: false);
                     hch.UseProxy = true;
                  }
            ));

         }

      #if NETFRAMEWORK
         ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
      #endif
      }

      [SetUp]
      public void BeforeEachTest()
      {
         client = new CoinbaseClient();
      }

      [Test]
      public async Task can_get_currencies()
      {
         var r = await client.Data.GetCurrenciesAsync();
         var usd = r.Data.First(c => "USD".Equals(c.Id));
         usd.Name.Should().StartWith("United States Dollar");
      }

      [Test]
      public async Task can_get_exchange_rates()
      {
         var r = await client.Data.GetExchangeRatesAsync("ETH");
         r.Data.Currency.Should().Be("ETH");
         r.Data.Rates["USD"].Should().BeGreaterThan(5);
      }

      [Test]
      public async Task can_get_buyprice()
      {
         var r = await client.Data.GetBuyPriceAsync("ETH-USD");
         r.Dump();
         r.Data.Amount.Should().BeGreaterThan(5);
         r.Data.Currency.Should().Be("USD");
         r.Data.Base.Should().Be("ETH");
      }

      [Test]
      public async Task can_get_sellprice()
      {
         var r = await client.Data.GetSellPriceAsync("ETH-USD");
         r.Dump();
         r.Data.Amount.Should().BeGreaterThan(5);
         r.Data.Currency.Should().Be("USD");
         r.Data.Base.Should().Be("ETH");
      }

      [Test]
      public async Task can_get_spotprice()
      {
         var r = await client.Data.GetSpotPriceAsync("ETH-USD");
         r.Dump();
         r.Data.Amount.Should().BeGreaterThan(5);
         r.Data.Currency.Should().Be("USD");
         r.Data.Base.Should().Be("ETH");
      }

      [Test]
      public async Task can_get_time()
      {
         var r = await client.Data.GetCurrentTimeAsync();
         r.Dump();
         r.Data.Iso.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromHours(1));
      }
   }
}
