using System;
using System.Configuration;
using System.Net;
using Coinbase.ObjectModel;
using Coinbase.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;

namespace Coinbase
{
	public static class CoinbaseConstants
	{
		public const string LiveApiUrl = "https://api.coinbase.com/v2/";
		public const string TestApiUrl = "https://api.sandbox.coinbase.com/v2/";
		public const string LiveCheckoutUrl = "https://coinbase.com/checkouts/{code}";
		public const string TestCheckoutUrl = "https://sandbox.coinbase.com/checkouts/{code}";
        public const string ApiVersionDate = "2015-11-17";
    }
    public class CoinbaseApi
    {
        internal readonly string apiKey;
        internal readonly string apiSecret;
	    internal readonly string apiUrl;
	    internal readonly string apiCheckoutUrl;
	    internal readonly WebProxy proxy;

        public JsonSerializerSettings JsonSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
				 
            };

        private bool useTimeApi;

        /// <summary>
        /// The main class for making Coinbase API calls.
        /// </summary>
        /// <param name="apiKey">Your API key</param>
        /// <param name="apiSecret">Your API secret</param>
        /// <param name="useTimeApi">Use Coinbase's Time API in signing requests. Results in 2 requests per call 
        /// (one to get time, one to send signed request). Uses coinbase server to prevent clock skew. If useTimeApi=false
        /// you must make sure your server time does not drift apart from Coinbase's server time. Read more here: 
        /// https://developers.coinbase.com/api/v2#api-key </param>
        public CoinbaseApi(string apiKey = "", string apiSecret = "", bool useSandbox = false, WebProxy proxy = null, bool useTimeApi = true) :
			this(apiKey, apiSecret, null, null, useTimeApi, proxy )
	    {
	        if( useSandbox )
	        {
	            this.apiUrl = CoinbaseConstants.TestApiUrl;
	            this.apiCheckoutUrl = CoinbaseConstants.TestCheckoutUrl;
	        }
	    }

        /// <summary>
		/// The main class for making Coinbase API calls.
		/// </summary>
		/// <param name="apiKey">Your API Key</param>
		/// <param name="apiSecret">Your API Secret </param>
		/// <param name="customApiEndpoint">A custom URL endpoint. Typically, you'd use this if you want to use the sandbox URL.</param>
		/// <param name="useTimeApi">Use Coinbase's Time API in signing requests. Results in 2 requests per call 
        /// (one to get time, one to send signed request). Uses coinbase server to prevent clock skew. If useTimeApi=false
        /// you must make sure your server time does not drift apart from Coinbase's server time. Read more here: 
        /// https://developers.coinbase.com/api/v2#api-key </param>
        public CoinbaseApi( 
			string apiKey,
			string apiSecret,
			string apiUrl = CoinbaseConstants.LiveApiUrl,
			string checkoutUrl = CoinbaseConstants.LiveCheckoutUrl,
            bool useTimeApi = true,
			WebProxy proxy = null)
        {
            //Issue #11 -- Check .NET's SecurityProtocol compatibility with Coinbase API Server
            //
            //For now, we'll keep this disabled until we know more. About the proper way to deal with SSL3
            //
            //if( ServicePointManager.SecurityProtocol == SecurityProtocolType.Ssl3 )
            //{
            //    throw new NotSupportedException(
            //        "ServicePointManager.SecurityProtocol is set to SSL3 which is not supported by Coinbase API Servers. Please configure ServicePointManager.SecurityProtocol to another value like TLS in your application startup. More information here: https://github.com/bchavez/Coinbase/issues/11");
            //}

            this.apiKey = !string.IsNullOrWhiteSpace( apiKey ) ? apiKey : ConfigurationManager.AppSettings["CoinbaseApiKey"];
            this.apiSecret = !string.IsNullOrWhiteSpace( apiSecret ) ? apiSecret : ConfigurationManager.AppSettings["CoinbaseApiSecret"];
            if ( string.IsNullOrWhiteSpace( this.apiKey ) || string.IsNullOrWhiteSpace(this.apiSecret) )
            {
                throw new ArgumentException( "The API key / secret must not be empty. A valid API key and API secret should be used in the CoinbaseApi constructor or an appSettings configuration element with <add key='CoinbaseApiKey' value='my_api_key' /> and <add key='CoinbaseApiSecret' value='my_api_secret' /> should exist.", "apiKey" );
            }

            this.apiUrl = !string.IsNullOrWhiteSpace(apiUrl) ? apiUrl : CoinbaseConstants.LiveApiUrl;
            this.apiCheckoutUrl = !string.IsNullOrWhiteSpace(checkoutUrl) ? checkoutUrl : CoinbaseConstants.LiveCheckoutUrl;

			this.proxy = proxy;
            this.useTimeApi = useTimeApi;
        }

        protected virtual RestClient CreateClient()
        {          
            var client = new RestClient( apiUrl )
	            {
		            Proxy = this.proxy,
					Authenticator = GetAuthenticator()
	            };

	        client.AddHandler( "application/json", new JsonNetDeseralizer( JsonSettings ) );
            return client;
        }

        protected virtual IAuthenticator GetAuthenticator()
        {
            return new CoinbaseApiAuthenticator(apiKey, apiSecret, useTimeApi, JsonSettings);
        }

        protected virtual IRestRequest CreateRequest( string action, Method method = Method.POST )
        {
            var post = new RestRequest(action, method)
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = new JsonNetSerializer(JsonSettings),
                };               

            return post;
        }

        /// <summary>
        /// Sends a Raw Json object model to the endpoint using an HTTP method.
        /// Recommended use is to use a JObject for the body or a serializable typesafe class.
        /// </summary>
        /// <param name="body">The JSON request body</param>
        /// <param name="endpoint">The API endpoint. Ex: /checkout, /orders, /time</param>
        /// <param name="httpMethod">The HTTP method to use. Default: POST.</param>
        public virtual CoinbaseResponse SendRequest(object body, string endpoint, Method httpMethod = Method.POST)
        {
            var client = CreateClient();

            var req = CreateRequest(endpoint, httpMethod)
                .AddJsonBody(body);

            var resp = client.Execute<CoinbaseResponse>(req);

            return resp.Data;
        }

        /// <summary>
        /// Sends a Raw Json object model to the endpoint using an HTTP method.
        /// Recommended use is to use a JObject for the body or a serializable typesafe class.
        /// </summary>
        /// <typeparam name="TResponse">Type T of CoinbaseResponse.Data</typeparam>
        /// <param name="body">The JSON request body</param>
        /// <param name="endpoint">The API endpoint. Ex: /checkout, /orders, /time</param>
        /// <param name="httpMethod">The HTTP method to use. Default: POST.</param>
        public virtual CoinbaseResponse<TResponse> SendRequest<TResponse>(object body, string endpoint, Method httpMethod = Method.POST)
        {
            var client = CreateClient();

            var req = CreateRequest(endpoint, httpMethod)
                .AddJsonBody(body);

            var resp = client.Execute<CoinbaseResponse<TResponse>>(req);

            return resp.Data;
        }

        /// <summary>
        /// Creates a new merchant order checkout product.
        /// All checkouts and subsequent orders created using this endpoint are created for merchant’s primary account.
        /// Using this endpoint to create checkouts and orders is useful when you want to build a merchant checkout experience with Coinbase’s merchant tools.
        /// </summary>
        public virtual CoinbaseResponse CreateCheckout(CheckoutRequest checkout)
        {
            return SendRequest(checkout, "checkouts");
        }

        /// <summary>
        /// Get the API server time.
        /// </summary>
        public virtual Time GetTime()
        {
            var resp = SendRequest<Time>(null, "time", Method.GET);
            return resp.Data;
        }

        /// <summary>
        /// Get the final checkout redirect URL from a CoinbaseResponse. The response
        /// from CreateCheckout() call should be used.
        /// </summary>
        /// <param name="checkoutResponse">The response from calling CreateCheckout()</param>
        /// <returns>The redirect URL for the customer checking out</returns>
        public virtual string GetCheckoutUrl(CoinbaseResponse checkoutResponse)
        {
            var id = checkoutResponse.Data["id"]?.ToString();
            if( string.IsNullOrWhiteSpace(id) )
                throw new ArgumentException("The checkout response must have an ID field. None was found.", nameof(checkoutResponse));

            return apiCheckoutUrl.Replace("{code}", id);
        }

        /// <summary>
        /// Gets a notification object from JSON.
        /// </summary>
        /// <param name="json">Received from Coinbase in the HTTP callback</param>
        /// <returns></returns>
        public virtual Notification GetNotification(string json)
        {
            return JsonConvert.DeserializeObject<Notification>(json, JsonSettings);
        }

    }
}