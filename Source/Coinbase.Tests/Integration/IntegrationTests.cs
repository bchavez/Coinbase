using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

         var webProxy = new WebProxy("http://localhost.:8888", BypassOnLocal: false);

         FlurlHttp.Configure(settings =>
            {
               settings.HttpClientFactory = new ProxyFactory(webProxy);
            });

      }

      protected void ReadSecrets()
      {
         var json = File.ReadAllText("../../.secrets.txt");
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
      public async Task can_get_auths()
      {
         client = new CoinbaseClient(new OAuthConfig { AccessToken = secrets.OAuthCode });
         var r = await client.Users.GetAuthInfoAsync();
         r.Dump();
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

      [Test]
      public async Task run_expired_token()
      {
         this.client = new CoinbaseClient(new OAuthConfig { AccessToken = secrets.OAuthAccessToken, RefreshToken = secrets.OAuthRefreshToken })
            .WithAutomaticOAuthTokenRefresh(secrets.OAuthClientId, secrets.OAuthClientSecret);

         var authInfo = await this.client.Users.GetAuthInfoAsync();
         authInfo.Dump();
      }
   }

   [Explicit]
   public class UserTests : IntegrationTests
   {
      protected CoinbaseClient client;

      public UserTests()
      {
         client = new CoinbaseClient(new ApiKeyConfig { ApiKey = secrets.ApiKey, ApiSecret = secrets.ApiSecret});
      }

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

      [Test]
      public async Task test_state()
      {
         var accounts = await client.Accounts.ListAccountsAsync();
         var ethAccount = accounts.Data.FirstOrDefault(x => x.Name == "ETH Wallet");
         var ethAddresses = await client.Addresses.ListAddressesAsync(ethAccount.Id);
         var ethAddress = ethAddresses.Data.FirstOrDefault();
         var ethTransactions = await client.Transactions.ListTransactionsAsync(ethAccount.Id);
      }
   }
}
