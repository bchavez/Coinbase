using Coinbase.Exceptions;
using Coinbase.ObjectModel;
using Coinbase.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Coinbase
{
    public abstract class CoinbaseApiBase
    {

        public JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        internal readonly WebProxy proxy;
        internal readonly string apiUrl;

        public CoinbaseApiBase(CoinbaseOptions options)
        {
            this.apiUrl = options.ApiUrl;
            if (string.IsNullOrEmpty(options.ApiUrl))
            {
                this.apiUrl = options.UseSandbox ? CoinbaseConstants.TestApiUrl : CoinbaseConstants.LiveApiUrl;
            }
        }

        protected virtual RestClient CreateClient()
        {
            var client = new RestClient(apiUrl)
            {
                Proxy = this.proxy,
                Authenticator = GetAuthenticator()
            };

            client.AddHandler("application/json", new JsonNetDeseralizer(JsonSettings));
            return client;
        }

        protected abstract IAuthenticator GetAuthenticator();

        protected virtual IRestRequest CreateRequest(string action, Method method = Method.POST)
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
        /// <param name="endpoint">The API endpoint. Ex: /checkout, /orders, /time</param>
        /// <param name="body">The JSON request body</param>
        /// <param name="httpMethod">The HTTP method to use. Default: POST.</param>
        public virtual CoinbaseResponse SendRequest(RequestOptions requestOptions)
        {
            return this.SendRequest<CoinbaseResponse, JToken>(requestOptions);
        }

        /// <summary>
        /// Sends a Raw Json object model to the endpoint using an HTTP method.
        /// Recommended use is to use a JObject for the body or a serializable typesafe class.
        /// </summary>
        /// <typeparam name="TResponse">Type T of CoinbaseResponse.Data</typeparam>
        public virtual CoinbaseResponse<TResponse> SendRequest<TResponse>(RequestOptions requestOptions)
        {
            return this.SendRequest<CoinbaseResponse<TResponse>, TResponse>(requestOptions);
        }

        /// <summary>
        /// Sends a Raw Json object model to the endpoint using an HTTP method.
        /// Recommended use is to use a JObject for the body or a serializable typesafe class.
        /// </summary>
        /// <param name="endpoint">The API endpoint. Ex: /checkout, /orders, /time</param>
        /// <param name="body">The JSON request body</param>
        /// <param name="httpMethod">The HTTP method to use. Default: POST.</param>
        public virtual CoinbaseResponse SendRequest(string endpoint, object body, Method httpMethod = Method.POST)
        {
            var client = CreateClient();

            var req = CreateRequest(endpoint, httpMethod)
                .AddJsonBody(body);

            return this.GetResponse<CoinbaseResponse, JToken>(client, req);
        }

        /// <summary>
        /// Sends a Raw Json object model to the endpoint using an HTTP method.
        /// Recommended use is to use a JObject for the body or a serializable typesafe class.
        /// </summary>
        /// <typeparam name="TResponse">Type T of CoinbaseResponse.Data</typeparam>
        /// <param name="endpoint">The API endpoint. Ex: /checkout, /orders, /time</param>
        /// <param name="body">The JSON request body</param>
        /// <param name="httpMethod">The HTTP method to use. Default: POST.</param>
        public virtual CoinbaseResponse<TResponse> SendRequest<TResponse>(string endpoint, object body, Method httpMethod = Method.POST)
        {
            var client = CreateClient();

            var req = CreateRequest(endpoint, httpMethod)
               .AddJsonBody(body);

            return this.GetResponse<TResponse>(client, req);
        }


        /// <summary>
        /// Sends a get request to the endpoint using GET HTTP method.
        /// </summary>
        /// <typeparam name="TResponse">Type T of CoinbaseResponse.Data</typeparam>
        /// <param name="endpoint">The API endpoint. Ex: /checkout, /orders, /time</param>
        /// <param name="queryParams">Query URL parameters to include in the GET request.</param>
        public virtual CoinbaseResponse<TResponse> SendGetRequest<TResponse>(string endpoint, params KeyValuePair<string, string>[] queryParams)
        {
            var client = CreateClient();

            var req = CreateRequest(endpoint, Method.GET);
            if (queryParams != null)
            {
                foreach (var kvp in queryParams)
                {
                    req.AddQueryParameter(kvp.Key, kvp.Value);
                }
            }

            return this.GetResponse<TResponse>(client, req);
        }

        /// <summary>
        /// Sends a Raw Json object model to the endpoint using an HTTP method.
        /// Recommended use is to use a JObject for the body or a serializable typesafe class.
        /// </summary>
        /// <typeparam name="TResponse">Type T of CoinbaseResponse Where T is CoinbaseResponse<TData></typeparam>
        /// <typeparam name="TData">Type T of Response Data</typeparam>
        public virtual TResponse SendRequest<TResponse, TData>(RequestOptions requestOptions)
            where TResponse : CoinbaseResponse<TData>, new()
        {
            var client = CreateClient();

            var req = CreateRequest(requestOptions.Endpoint, requestOptions.HttpMethod);
            requestOptions.Parameters.ForEach(param => req.AddOrUpdateParameter(param));

            return this.GetResponse<TResponse, TData>(client, req);
        }

        protected CoinbaseResponse<TData> GetResponse<TData>(RestClient client, IRestRequest req)
        {
            var resp = client.Execute<CoinbaseResponse<TData>>(req);
            this.HandleKnownExceptions(resp);
            return resp.Data;
        }

        protected TResponse GetResponse<TResponse, TData>(RestClient client, IRestRequest req)
            where TResponse : CoinbaseResponse<TData>, new()
        {
            var resp = client.Execute<TResponse>(req);
            this.HandleKnownExceptions(resp);
            return resp.Data;
        }

        //TODO: Create Error Handler
        protected void HandleKnownExceptions(IRestResponse response)
        {
            IDeserializer deserializer = null;
            ErrorResponse errorResponse = null;
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    deserializer = new JsonNetDeseralizer(JsonSettings);
                    errorResponse = deserializer.Deserialize<ErrorResponse>(response);
                    var expiredToken = errorResponse.Errors.FirstOrDefault(e => e.Id == "expired_token");
                    if (expiredToken != null)
                    {
                        throw new TokenExpiredException(response.StatusCode, errorResponse);
                    }
                    throw new CoinbaseApiException(response.StatusCode, errorResponse);
                case HttpStatusCode.Forbidden:
                    deserializer = new JsonNetDeseralizer(JsonSettings);
                    errorResponse = deserializer.Deserialize<ErrorResponse>(response);
                    var invalidScope = errorResponse.Errors.FirstOrDefault(e => e.Id == "invalid_scope");
                    if (invalidScope != null)
                    {
                        throw new InvalidScopeException(response.StatusCode, errorResponse);
                    }
                    throw new CoinbaseApiException(response.StatusCode, errorResponse);
                case HttpStatusCode.PaymentRequired:
                    deserializer = new JsonNetDeseralizer(JsonSettings);
                    errorResponse = deserializer.Deserialize<ErrorResponse>(response);
                    var twoFactorRequired = errorResponse.Errors.FirstOrDefault(e => e.Id == "two_factor_required");
                    if (twoFactorRequired != null)
                    {
                        throw new Transaction2FaRequiredException(response.StatusCode, errorResponse);
                    }
                    throw new CoinbaseApiException(response.StatusCode, errorResponse);
            }
        }
    }
}
