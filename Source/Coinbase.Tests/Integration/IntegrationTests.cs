using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.Models;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using Z.ExtensionMethods;

namespace Coinbase.Tests.Integration
{
   public class Secrets
   {
      public string ApiKey { get; set; }
      public string ApiSecret { get; set; }
      public string OAuthClientId { get; set; }
      public string OAuthClientSecret { get; set; }
      public string OAuthCode { get; set; }
      public string OAuthAccessToken { get; set; }
      public string OAuthRefreshToken { get; set; }
   }

   [Explicit]
   public class IntegrationTests
   {
      protected Secrets secrets;

      public IntegrationTests()
      {
         Directory.SetCurrentDirectory(Path.GetDirectoryName(typeof(IntegrationTests).Assembly.Location));

         ReadSecrets();

         FlurlHttp.Clients.WithDefaults(builder => builder.ConfigureInnerHandler(
            hch =>
               {
                  hch.Proxy = new WebProxy("http://localhost.:8888", BypassOnLocal: false);
                  hch.UseProxy = true;
               }
         ));
      }

      protected void ReadSecrets()
      {
         var json = File.ReadAllText("../../../.secrets.txt");
         this.secrets = JsonConvert.DeserializeObject<Secrets>(json);
      }
   }

   [Explicit]
   public class OAuthTests : IntegrationTests
   {
      private CoinbaseClient client;

      private string redirectUrl = "http://localhost:8080/callback";

      public OAuthTests()
      {
         
      }

      [Test]
      public async Task test_full_flow_and_expiration()
      {
         //client.

         var opts = new AuthorizeOptions
            {
               ClientId = secrets.OAuthClientId,
               RedirectUri = redirectUrl,
               State = "random",
               Scope = "wallet:accounts:read"
            };

         var authUrl = OAuthHelper.GetAuthorizeUrl(opts);
         authUrl.Dump();

         ("Execute the following URL and authorize the app. " +
            "Then, copy the callback?code=VALUE value to the secrets file.").Dump();
      }

      [Test]
      public async Task convert_code_to_token()
      {
         var token = await OAuthHelper.GetAccessTokenAsync(secrets.OAuthCode, secrets.OAuthClientId, secrets.OAuthClientSecret, redirectUrl);
         token.Dump();
      }
   }
}
