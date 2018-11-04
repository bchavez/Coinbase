using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Coinbase
{
    public class CoinbaseOptions
    {
        /// <summary>
        /// Coinbase Api Endpint
        /// </summary>
        public string ApiUrl { get; set; } = CoinbaseConstants.LiveApiUrl;

        /// <summary>
        /// Flag to use Test Endpoints
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Web Proxy for to use in the Coinbase Api
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// Use Coinbase's Time API in signing requests. Results in 2 requests per call 
        /// (one to get time, one to send signed request). Uses coinbase server to prevent clock skew. If useTimeApi=false
        /// you must make sure your server time does not drift apart from Coinbase's server time. Read more here: 
        /// https://developers.coinbase.com/api/v2#api-key
        public bool UseTimeApi { get; set; } = true;
    }

    public class CoinbaseApiOptions : CoinbaseOptions
    {
        /// <summary>
        /// Coinbase Checkout Endpoint
        /// </summary>
        public string CheckoutUrl { get; set; } = CoinbaseConstants.LiveCheckoutUrl;

        /// <summary>
        /// Api Key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Api Secret
        /// </summary>
        public string ApiSecret { get; set; }
    }

    public class CoinbaseOAuthOptions : CoinbaseOptions
    {

        /// <summary>
        /// OAuth Access Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// OAuth Refresh Token
        /// </summary>
        public string RefreshToken { get; set; }
    }

}
