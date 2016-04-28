using System;
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
    /// <summary>
    /// 
    /// </summary>
    public class CoinbaseApiAuthenticator : IAuthenticator
    {
        private static int? diff = null;

        private readonly string apiKey;
        private readonly string apiSecret;
        private readonly bool useTimeApi;
        private readonly JsonSerializerSettings jsonSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="apiSecret"></param>
        /// <param name="useTimeApi"></param>
        /// <param name="jsonSettings"></param>
        public CoinbaseApiAuthenticator(string apiKey, string apiSecret, bool useTimeApi, JsonSerializerSettings jsonSettings)
        {
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.useTimeApi = useTimeApi;
            this.jsonSettings = jsonSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        public void Authenticate(IRestClient client, IRestRequest request)
        {
            var uri = client.BuildUri(request);
            var path = uri.PathAndQuery;

            if (path.EndsWith("/time") && path.Length <= 8)
            {
                request.AddHeader("CB-VERSION", CoinbaseConstants.ApiVersionDate);
                return;
            }
            string timestamp = null;

            if (useTimeApi)
            {
                if (diff != null)
                {
                    timestamp = (GetCurrentUnixTimestampSeconds() + diff.Value).ToString(CultureInfo.InvariantCulture);
                }
                else {

                    var timeReq = new RestRequest("/time", Method.GET)
                    {
                        JsonSerializer = new JsonNetSerializer(jsonSettings)
                    };


                    DateTime start = DateTime.UtcNow;
                    var timeResp = client.Execute<CoinbaseResponse<Time>>(timeReq);
                    int duration = (int)(DateTime.UtcNow - start).TotalSeconds;

                    if (timeResp.ErrorException != null && timeResp.Content.Contains("Attention Required! | CloudFlare"))
                    {
                        timestamp = GetCurrentUnixTimestampSeconds().ToString(CultureInfo.InvariantCulture);
                    }
                    else {

                        ulong epoch = timeResp.Data.Data.Epoch;

                        // Store the difference to prevent querying the API on each request.
                        // Only if duration is on the safe side. < 5s is acceptable.
                        if (duration < 5)
                        {
                            diff = (int)(epoch - (ulong)DateTimeTounixTimestampSeconds(start));
                        }

                        timestamp = epoch.ToString();
                    }
                }
            }
            else
            {
                timestamp = GetCurrentUnixTimestampSeconds().ToString(CultureInfo.InvariantCulture);
            }

            var method = request.Method.ToString().ToUpper(CultureInfo.InvariantCulture);

            var body = string.Empty;

            var param = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            if (param != null && param?.Value?.ToString() != "null" && !string.IsNullOrWhiteSpace(param?.Value?.ToString()))
                body = param.Value.ToString();

            var hmacSig = GenerateSignature(timestamp, method, path, body, this.apiSecret);

            request.AddHeader("CB-ACCESS-KEY", this.apiKey)
                .AddHeader("CB-ACCESS-SIGN", hmacSig)
                .AddHeader("CB-ACCESS-TIMESTAMP", timestamp)
                .AddHeader("CB-VERSION", CoinbaseConstants.ApiVersionDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string GenerateSignature(string timestamp, string method, string url, string body, string appSecret)
        {
            return GetHMACInHex(appSecret, timestamp + method + url + body);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static string GetHMACInHex(string key, string data)
        {
            var hmacKey = Encoding.UTF8.GetBytes(key);

            using (var signatureStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var hex = new HMACSHA256(hmacKey).ComputeHash(signatureStream)
                    .Aggregate(new StringBuilder(), (sb, b) => sb.AppendFormat("{0:x2}", b), sb => sb.ToString());

                return hex;
            }
        }



        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>
        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentUnixTimestampSeconds()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long DateTimeTounixTimestampSeconds(DateTime dt)
        {
            return (long)(dt - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }
    }
}