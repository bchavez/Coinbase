using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Coinbase.ObjectModel;
using Coinbase.Serialization;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Coinbase
{
   public class CoinbaseApiAuthenticator : IAuthenticator
   {
      private readonly string apiKey;
      private readonly string apiSecret;
      private readonly bool useTimeApi;
      private readonly JsonSerializerSettings jsonSettings;

      public CoinbaseApiAuthenticator(string apiKey, string apiSecret, bool useTimeApi, JsonSerializerSettings jsonSettings)
      {
         this.apiKey = apiKey;
         this.apiSecret = apiSecret;
         this.useTimeApi = useTimeApi;
         this.jsonSettings = jsonSettings;
      }


      public void Authenticate(IRestClient client, IRestRequest request)
      {
         var uri = client.BuildUri(request);
         var path = uri.AbsolutePath;

         if( path.EndsWith("/time") && path.Length <= 8 )
         {
            request.AddHeader("CB-VERSION", CoinbaseConstants.ApiVersionDate);
            return;
         }
         string timestamp = null;
         if( useTimeApi )
         {
            var timeReq = new RestRequest("/time", Method.GET)
               {
                  JsonSerializer = new JsonNetSerializer(jsonSettings)
               };

            var timeResp = client.Execute<CoinbaseResponse<Time>>(timeReq);
            timestamp = timeResp.Data.Data.Epoch.ToString();
         }
         else
         {
            timestamp = GetCurrentUnixTimestampSeconds().ToString(CultureInfo.InvariantCulture);
         }

         var method = request.Method.ToString().ToUpper(CultureInfo.InvariantCulture);

         var body = string.Empty;
         if( request.Method != Method.GET )
         {
            var param = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            if( param != null && param?.Value?.ToString() != "null" && !string.IsNullOrWhiteSpace(param?.Value?.ToString()) )
               body = param.Value.ToString();
         }
         else
         {
            path = uri.PathAndQuery;
         }


         var hmacSig = GenerateSignature(timestamp, method, path, body, this.apiSecret);

         request.AddHeader("CB-ACCESS-KEY", this.apiKey)
            .AddHeader("CB-ACCESS-SIGN", hmacSig)
            .AddHeader("CB-ACCESS-TIMESTAMP", timestamp)
            .AddHeader("CB-VERSION", CoinbaseConstants.ApiVersionDate);
      }

      public static string GenerateSignature(string timestamp, string method, string url, string body, string appSecret)
      {
         return GetHMACInHex(appSecret, timestamp + method + url + body);
      }

      internal static string GetHMACInHex(string key, string data)
      {
         var hmacKey = Encoding.UTF8.GetBytes(key);

         using( var signatureStream = new MemoryStream(Encoding.UTF8.GetBytes(data)) )
         {
            var hex = new HMACSHA256(hmacKey).ComputeHash(signatureStream)
               .Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat("{0:x2}", b), sb => sb.ToString());

            return hex;
         }
      }


      private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

      public static long GetCurrentUnixTimestampMillis()
      {
         return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
      }

      public static DateTime DateTimeFromUnixTimestampMillis(long millis)
      {
         return UnixEpoch.AddMilliseconds(millis);
      }

      public static long GetCurrentUnixTimestampSeconds()
      {
         return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
      }

      public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
      {
         return UnixEpoch.AddSeconds(seconds);
      }
   }
}