using Flurl.Http.Testing;
using NUnit.Framework;
using System;

namespace Coinbase.Tests
{
   public class ServerTest
   {
      protected HttpTest server;

      [SetUp]
      public virtual void BeforeEachTest()
      {
         server = new HttpTest();
      }

      [TearDown]
      public virtual void AfterEachTest()
      {
         this.server.Dispose();
      }
      
      protected void SetupServerPagedResponse(string pageJson, string dataJson)
      {
         var json = @"{
    ""pagination"": {pageJson},
    ""data"": [{dataJson}]
}
".Replace("{dataJson}", dataJson)
            .Replace("{pageJson}", pageJson);

         server.RespondWith(json);

         Console.WriteLine("Anticipated server response:");
         Console.WriteLine("---");
         Console.WriteLine();
         Console.WriteLine(json);
         Console.WriteLine();
         Console.WriteLine("---");
      }

      protected void SetupServerSingleResponse(string dataJson)
      {
         var json = @"{
    ""data"": {dataJson}
}
".Replace("{dataJson}", dataJson);

         server.RespondWith(json);

         Console.WriteLine("Anticipated server response:");
         Console.WriteLine("---");
         Console.WriteLine();
         Console.WriteLine(json);
         Console.WriteLine();
         Console.WriteLine("---");
      }
   }

   public class OAuthServerTest : ServerTest
   {
      protected CoinbaseClient client;

      public const string OauthKey = "369ECD3F-2D00-4D7A-ACDB-92C2DC35A878";

      [SetUp]
      public override void BeforeEachTest()
      {
         base.BeforeEachTest();
         client = new CoinbaseClient(new OAuthConfig{AccessToken = OauthKey});
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
            .WithHeader("User-Agent", CoinbaseClient.UserAgent)
            .WithHeader("Authorization", $"Bearer {OauthKey}");
      }
   }

}
