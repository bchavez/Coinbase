using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Flurl.Http.Testing;
using NUnit.Framework;

namespace Coinbase.Tests.OAuthTests
{
   public class OAuthHelperTests
   {
      private HttpTest server;

      [SetUp]
      public void BeforeEachTest()
      {
         server = new HttpTest();
      }

      [TearDown]
      public void AfterEachTest()
      {
         server.Dispose();
      }


      [Test]
      public async Task can_create_authorization_url()
      {
         var opts = new AuthorizeOptions
         {
            ClientId = "YOUR_CLIENT_ID",
            RedirectUri = "YOUR_REDIRECT_URL",
            State = "SECURE_RANDOM",
            Scope = "wallet:accounts:read"
         };
         var authUrl = OAuthHelper.GetAuthorizeUrl(opts);

         var url =
            "https://www.coinbase.com/oauth/authorize?response_type=code&client_id=YOUR_CLIENT_ID&redirect_uri=YOUR_REDIRECT_URL&state=SECURE_RANDOM&scope=wallet%3Aaccounts%3Aread";


         var response = await authUrl.GetAsync();

         server.ShouldHaveExactCall(url);

      }

      [Test]
      public async Task can_revoke_token()
      {
         server.RespondWith("");

         var x  = await OAuthHelper.RevokeTokenAsync("fff", "vvv");

         server.ShouldHaveExactCall("https://api.coinbase.com/oauth/revoke")
            .WithVerb(HttpMethod.Post)
            .WithRequestBody("token=fff&access_token=vvv");
      }


      [Test]
      public async Task can_get_access_token()
      {
         var tokenResponse = @"{
""access_token"":""aaa"",
""token_type"":""bearer"",
""expires_in"":7200,
""refresh_token"":""bbb"",
""scope"":""wallet:user:read wallet:accounts:read"",
""created_at"":1542649114
}";
         server.RespondWith(tokenResponse);

         var token = await OAuthHelper.GetAccessTokenAsync(
            code:"4c666b5c0c0d9d3140f2e0776cbe245f3143011d82b7a2c2a590cc7e20b79ae8",
            clientId:"1532c63424622b6e9c4654e7f97ed40194a1547e114ca1c682f44283f39dfa49",
            clientSecret:"3a21f08c585df35c14c0c43b832640b29a3a3a18e5c54d5401f08c87c8be0b20",
            redirectUri: "http://localhost:8080/callback");
         
         server.ShouldHaveExactCall("https://api.coinbase.com/oauth/token")
            .WithVerb(HttpMethod.Post)
            .WithRequestBody("grant_type=authorization_code" +
                             "&code=4c666b5c0c0d9d3140f2e0776cbe245f3143011d82b7a2c2a590cc7e20b79ae8" +
                             "&client_id=1532c63424622b6e9c4654e7f97ed40194a1547e114ca1c682f44283f39dfa49" +
                             "&client_secret=3a21f08c585df35c14c0c43b832640b29a3a3a18e5c54d5401f08c87c8be0b20" +
                             $"&redirect_uri={Url.Encode("http://localhost:8080/callback")}");

         token.AccessToken.Should().Be("aaa");
         token.TokenType.Should().Be("bearer");
         token.ExpiresInSeconds.Should().Be(7200);
         token.Expires.TotalHours.Should().Be(2);
         token.CreatedAt.Date.Should().Be(DateTime.Parse("11/19/2018", CultureInfo.InvariantCulture));
         token.RefreshToken.Should().Be("bbb");
         token.Scope.Should().Be("wallet:user:read wallet:accounts:read");
      }


      [Test]
      public async Task can_refresh_token()
      {
         var tokenResponse = @"{
""access_token"":""aaa"",
""token_type"":""bearer"",
""expires_in"":7200,
""refresh_token"":""bbb"",
""scope"":""all"",
""created_at"":1542649114
}";

         server.RespondWith(tokenResponse);

         var token = await OAuthHelper.RenewAccessAsync("refresh", "clientid", "clientsecret");

         server.ShouldHaveExactCall("https://api.coinbase.com/oauth/token")
            .WithVerb(HttpMethod.Post)
            .WithRequestBody("grant_type=refresh_token" +
                             "&refresh_token=refresh" +
                             "&client_id=clientid" +
                             "&client_secret=clientsecret");

         token.AccessToken.Should().Be("aaa");
         token.TokenType.Should().Be("bearer");
         token.ExpiresInSeconds.Should().Be(7200);
         token.Expires.TotalHours.Should().Be(2);
         token.CreatedAt.Date.Should().Be(DateTime.Parse("11/19/2018", CultureInfo.InvariantCulture));
         token.RefreshToken.Should().Be("bbb");
         token.Scope.Should().Be("all");
      }
   }
}
