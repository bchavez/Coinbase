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
      [Test]
      public async Task auto_refresh_token()
      {
         //simulate the expired response.
         var expiredResponse = @"{""errors"":[{""id"":""expired_token"",""message"":""The access token is expired""}]}";
         server.RespondWith(expiredResponse, status:401);


         //simulate the refresh response.
         var refreshResponse = @"{
""access_token"":""aaa"",
""token_type"":""bearer"",
""expires_in"":7200,
""refresh_token"":""bbb"",
""scope"":""all"",
""created_at"":1542649114
}";
         server.RespondWith(refreshResponse, status: 200);

         //simulate the actual successful response after token refresh.
         SetupServerSingleResponse(Examples.User);

         //enable automatic refresh
         client.WithAutomaticOAuthTokenRefresh("clientId", "clientSecret");

         var response = await client.Users.GetCurrentUserAsync();

         response.Dump();

         var config = client.Config as OAuthConfig;
         config.AccessToken.Should().Be("aaa");
         config.RefreshToken.Should().Be("bbb");

         Examples.UserModel.Should().BeEquivalentTo(response.Data);

         server.ShouldHaveMadeACall()
            .WithHeader(HeaderNames.Version, CoinbaseClient.ApiVersionDate)
            .WithHeader("User-Agent", CoinbaseClient.UserAgent)
            .WithHeader("Authorization", $"Bearer aaa");
      }

      [Test]
      public async Task auto_refresh_token_with_callback()
      {
         //simulate the expired response.
         var expiredResponse = @"{""errors"":[{""id"":""expired_token"",""message"":""The access token is expired""}]}";
         server.RespondWith(expiredResponse, status: 401);

         //simulate the refresh response.
         var refreshResponse = @"{
""access_token"":""aaa"",
""token_type"":""bearer"",
""expires_in"":7200,
""refresh_token"":""bbb"",
""scope"":""all"",
""created_at"":1542649114
}";
         server.RespondWith(refreshResponse, status: 200);

         //simulate the actual successful response after token refresh.
         SetupServerSingleResponse(Examples.User);

         //enable automatic refresh
         client.WithAutomaticOAuthTokenRefresh("clientId", "clientSecret", o =>
            {
               o.AccessToken.Should().Be("aaa");
               o.RefreshToken.Should().Be("bbb");
               return Task.FromResult(0);
            });

         var response = await client.Users.GetCurrentUserAsync();

         response.Dump();

         var config = client.Config as OAuthConfig;
         config.AccessToken.Should().Be("aaa");
         config.RefreshToken.Should().Be("bbb");

         Examples.UserModel.Should().BeEquivalentTo(response.Data);

         server.ShouldHaveMadeACall()
            .WithHeader(HeaderNames.Version, CoinbaseClient.ApiVersionDate)
            .WithHeader("User-Agent", CoinbaseClient.UserAgent)
            .WithHeader("Authorization", $"Bearer aaa");
      }

   }
}
