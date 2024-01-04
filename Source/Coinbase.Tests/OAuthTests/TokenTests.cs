using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Coinbase.Tests.OAuthTests
{
   public class TokenTests : ServerTest
   {
      private CoinbaseClient client;

      [SetUp]
      public void BeforeEachTest()
      {
         client = new CoinbaseClient(new OAuthConfig(){ AccessToken = "zzz"});
      }

   }
}
