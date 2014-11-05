using System;
using System.Configuration;
using System.Net;
using Coinbase.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace Coinbase
{
    public class CoinbaseApi
    {
        private readonly string apiKey;
        private readonly string apiSecret;

        private JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        public const string BaseUrl = "https://coinbase.com/api/v1/{action}?api_key={apikey}";
        public const string CheckoutPageUrl = "https://coinbase.com/checkouts/{code}";

        public CoinbaseApi() : this(string.Empty, string.Empty)
        {
        }

        [Obsolete( "Simple API Keys are being deprecated in favor of the new API Key + Secret system. Read more in the API docs.", true )]
        public CoinbaseApi(string apiSimpleKey)
        {
            throw new InvalidOperationException( "Simple API Keys are being deprecated in favor of the new API Key + Secret system. Read more in the API docs." );
        }

        public CoinbaseApi( string apiKey, string apiSecret )
        {
            this.apiKey = !string.IsNullOrWhiteSpace( apiKey ) ? apiKey : ConfigurationManager.AppSettings["CoinbaseApiKey"];
            this.apiSecret = !string.IsNullOrWhiteSpace( apiSecret ) ? apiSecret : ConfigurationManager.AppSettings["CoinbaseApiSecret"];
            if ( string.IsNullOrWhiteSpace( this.apiKey ) || string.IsNullOrWhiteSpace(this.apiSecret) )
            {
                throw new ArgumentException( "The API key / secret must not be empty. A valid API key and API secret should be used in the CoinbaseApi constructor or an appSettings configuration element with <add key='CoinbaseApiKey' value='my_api_key' /> and <add key='CoinbaseApiSecret' value='my_api_secret' /> should exist.", "apiKey" );
            }
        }

        public CoinbaseApi(string apiKey, string apiSecret, JsonSerializerSettings settings) : this(apiKey, apiSecret)
        {
            this.settings = settings;
        }

        protected virtual RestClient CreateClient()
        {
            const string BaseUri = "https://coinbase.com/api/v1/";
            
            var client = new RestClient( BaseUri );
#if DEBUG
            client.Proxy = new WebProxy( "http://localhost.:8888", false );
#endif
            client.Authenticator = GetAuthenticator();
            client.AddHandler( "application/json", new JsonNetDeseralizer( settings ) );
            return client;
        }

        protected virtual IAuthenticator GetAuthenticator()
        {
            return new CoinbaseApiAuthenticator(apiKey, apiSecret);
        }

        protected virtual IRestRequest CreateRequest( string action )
        {
            var post = new RestRequest(action, Method.POST)
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = new JsonNetSerializer(settings),
                };               

            return post;
        }

        /// <summary>
        /// Authenticated resource that creates a payment button, page, or iFrame to accept bitcoin on your website. This can be used to accept bitcoin for an individual item or to integrate with your existing shopping cart solution. For example, you could create a new payment button for each shopping cart on your website, setting the total and order number in the button at checkout.
        ///
        /// The code param returned in the response can be used to generate an embeddable button, payment page, or iFrame.
        /// </summary>
        public virtual ButtonResponse RegisterButton(ButtonRequest buttonRequest)
        {
            var client = CreateClient();

            var post = CreateRequest( "buttons" )
                .AddBody( buttonRequest );
            
            var resp = client.Execute<ButtonResponse>( post );

            if ( resp.ErrorException != null )
                throw resp.ErrorException;
            if ( resp.ErrorMessage != null )
                throw new Exception(resp.ErrorMessage);

            return resp.Data;
        }

        /// <summary>
        /// Authenticated resource which lets you generate an order associated with a button. After generating an order, you can send bitcoin to the address associated with the order to complete the order. The status of this newly created order will be ‘new’.
        /// </summary>
        public OrderResponse CreateOrder( ButtonResponse response )
        {
            var code = response.Button.Code;

            var client = CreateClient();

            var post = CreateRequest( "buttons/{code}/create_order" )
                .AddUrlSegment( "code", code );
                

            var resp = client.Execute<OrderResponse>(post);

            if ( resp.ErrorException != null )
                throw resp.ErrorException;

            return resp.Data;
        }
    }

}
