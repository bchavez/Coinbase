using System;

namespace Coinbase
{
   public class Config
   {
      internal Config()
      {
      }

      public string ApiUrl { get; set; } = CoinbaseApiBase.Endpoint;
      public bool UseTimeApi { get; set; } = true;

      internal virtual void EnsureValid()
      {
      }
   }
   public class OAuthConfig : Config
   {
      public string OAuthToken { get; set; }
      internal override void EnsureValid()
      {
         if (string.IsNullOrWhiteSpace(OAuthToken)) throw new ArgumentNullException(nameof(OAuthToken), "The OAuthToken must be specified.");
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
   }
}