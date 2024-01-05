using NUnit.Framework;

namespace Coinbase.Tests
{
   public class CoinbaseApiKeyTests : ServerTest
   {
      public string apiKey = "DBBD0428-B818-4F53-A5F4-F553DC4C374C";
      private CoinbaseClient client;

      [SetUp]
      public override void BeforeEachTest()
      {
         base.BeforeEachTest();
         client = new CoinbaseClient(new ApiKeyConfig{ ApiKey = "", ApiSecret = ""});
      }

      [TearDown]
      public override void AfterEachTest()
      {
         EnsureEveryRequestHasCorrectHeaders();
         base.AfterEachTest();
      }

      private void EnsureEveryRequestHasCorrectHeaders()
      {
         server.ShouldHaveMadeACall()
            .WithHeader(HeaderNames.Version, CoinbaseClient.ApiVersionDate)
            .WithHeader(HeaderNames.AccessKey, apiKey)
            .WithHeader("User-Agent", CoinbaseClient.UserAgent);
      }
   }
}
