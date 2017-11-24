using FluentAssertions;
using NUnit.Framework;

namespace Coinbase.Tests
{
   [TestFixture]
   public class ApiTests
   {
      [Test]
      public void ConstructorTests()
      {
         var api = new CoinbaseApi("0CCAA858-2625-4652-A781-9BF48A3E7635", "7DC36573-3C21-4783-AC17-1687AFAFB8C9", useSandbox: false, proxy: null);

         api.apiKey.Should().Be("0CCAA858-2625-4652-A781-9BF48A3E7635");
         api.apiSecret.Should().Be("7DC36573-3C21-4783-AC17-1687AFAFB8C9");

         api.apiUrl.Should().Be(CoinbaseConstants.LiveApiUrl);
         api.apiCheckoutUrl.Should().Be(CoinbaseConstants.LiveCheckoutUrl);

         var api2 = new CoinbaseApi("0CCAA858-2625-4652-A781-9BF48A3E7635", "7DC36573-3C21-4783-AC17-1687AFAFB8C9", useSandbox: true, proxy: null);

         api2.apiUrl.Should().Be(CoinbaseConstants.TestApiUrl);
         api2.apiCheckoutUrl.Should().Be(CoinbaseConstants.TestCheckoutUrl);
      }
   }
}