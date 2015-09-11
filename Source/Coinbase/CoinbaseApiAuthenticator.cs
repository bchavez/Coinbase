using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;

namespace Coinbase
{
    public class CoinbaseApiAuthenticator : IAuthenticator
    {
        private readonly string apiKey;
        private readonly string apiSecret;

        public CoinbaseApiAuthenticator(string apiKey, string apiSecret)
        {
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            var nonce = GetNonce();

            var url = client.BuildUri(request);

            var body = string.Empty;

            var param = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            if( param != null )
                body = param.Value.ToString();

            var hmacSig = GenerateSignature(nonce, url.ToString(), body, this.apiSecret);

            request.AddHeader("ACCESS_KEY", this.apiKey)
                .AddHeader("ACCESS_NONCE", nonce)
                .AddHeader("ACCESS_SIGNATURE", hmacSig);
        }

        /// <summary>
        /// The nonce is a positive integer number that must increase with every request you make.
        /// The ACCESS_SIGNATURE header is a HMAC-SHA256 hash of the nonce concatentated with the full URL and body of the HTTP request, encoded using your API secret.
        /// In some distributed scenarios, it might be necessary to override the implementation of this method to allow cluster of computers to make individual payment requests to coinbase
        /// where synchronization of the nonce is necessary.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetNonce()
        {
            return DateTime.UtcNow.Ticks.ToString();
        }


        public static string GenerateSignature( string nonce, string url, string body, string appSecret )
        {
            return GetHMACInHex( appSecret, nonce + url + body );
        }

        internal static string GetHMACInHex( string key, string data )
        {
            var hmacKey = Encoding.UTF8.GetBytes( key );

            using( var signatureStream = new MemoryStream( Encoding.UTF8.GetBytes( data ) ) )
            {
                var hex = new HMACSHA256( hmacKey ).ComputeHash( signatureStream )
                    .Aggregate( new StringBuilder(), ( sb, b ) => sb.AppendFormat( "{0:x2}", b ), sb => sb.ToString() );

                return hex;
            }
        }
    }
}