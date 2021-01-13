using Coinbase.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace Coinbase
{
   public class Config
   {
      internal Config()
      {
      }

      public string ApiUrl { get; set; } = CoinbaseClient.Endpoint;
      public bool UseTimeApi { get; set; } = true;

      internal virtual void EnsureValid()
      {
      }

      protected internal virtual void Configure(CoinbaseClient client)
      {

      }
   }

   public class OAuthConfig : Config
   {
      public string AccessToken { get; set; }
      public string RefreshToken { get; set; }

      internal override void EnsureValid()
      {
         if (string.IsNullOrWhiteSpace(this.AccessToken))
            throw new ArgumentNullException(nameof(this.AccessToken), $"The {nameof(AccessToken)} must be specified.");
      }

      protected internal override void Configure(CoinbaseClient client)
      {
         client.Configure(settings => UseOAuth(settings, client));
      }

      private void UseOAuth(ClientFlurlHttpSettings settings, CoinbaseClient client)
      {
         async Task ApplyAuthorization(FlurlCall call)
         {
            call.Request.WithOAuthBearerToken(this.AccessToken);
         }

         settings.BeforeCallAsync = ApplyAuthorization;
      }
   }

   public class ApiKeyConfig : Config
   {
      public string ApiKey { get; set; }
      public string ApiSecret { get; set; }
      internal override void EnsureValid()
      {
         if (string.IsNullOrWhiteSpace(ApiKey)) throw new ArgumentNullException(nameof(ApiKey), "The API Key must be specified.");
         if (string.IsNullOrWhiteSpace(ApiSecret)) throw new ArgumentNullException(nameof(ApiSecret), "The API Key must be specified.");
      }

      protected internal override void Configure(CoinbaseClient client)
      {
         client.Configure(settings => ApiKeyAuth(settings, client));
      }

      private void ApiKeyAuth(ClientFlurlHttpSettings settings, CoinbaseClient client)
      {
         async Task SetHeaders(FlurlCall http)
         {
            var body = http.RequestBody;
            var method = http.Request.Verb.Method.ToUpperInvariant();
            var url = http.Request.Url.ToUri().PathAndQuery;

            string timestamp;
            if (this.UseTimeApi)
            {
               var timeResult = await client.Data.GetCurrentTimeAsync().ConfigureAwait(false);
               timestamp = timeResult.Data.Epoch.ToString();
            }
            else
            {
               timestamp = TimeHelper.GetCurrentUnixTimestampSeconds().ToString(CultureInfo.CurrentCulture);
            }

            var signature = ApiKeyAuthenticator.GenerateSignature(timestamp, method, url, body, this.ApiSecret);

            http.Request
               .WithHeader(HeaderNames.AccessKey, this.ApiKey)
               .WithHeader(HeaderNames.AccessSign, signature)
               .WithHeader(HeaderNames.AccessTimestamp, timestamp);
         }

         settings.BeforeCallAsync = SetHeaders;
      }
   }
}
