using System;
using System.Collections.Generic;
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
        public const string CBVersionHeader = "CB-VERSION";
    }

    public class CoinbaseApi : CoinbaseApiBase
    {
        internal readonly string apiKey;
        internal readonly string apiSecret;
        internal readonly string apiCheckoutUrl;

        private bool useTimeApi;

        /// <summary>
        /// The main class for making Coinbase API calls.
        /// </summary>
        public CoinbaseApi(CoinbaseApiOptions options) : base(options)
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

            if (options.UseSandbox)
            {
                this.apiCheckoutUrl = CoinbaseConstants.TestCheckoutUrl;
            }
            if (string.IsNullOrWhiteSpace(options.ApiKey))
            {
                throw new ArgumentException("The API key must be specified.", nameof(options.ApiKey));
            }
            if (string.IsNullOrWhiteSpace(options.ApiSecret))
            {
                throw new ArgumentException("The API secret must be specified.", nameof(options.ApiSecret));
            }

            this.apiKey = options.ApiKey;
            this.apiSecret = options.ApiSecret;
            this.apiCheckoutUrl = !string.IsNullOrWhiteSpace(options.CheckoutUrl) ? 
                options.CheckoutUrl : CoinbaseConstants.LiveCheckoutUrl;
            this.useTimeApi = options.UseTimeApi;
        }

        protected override IAuthenticator GetAuthenticator()
        {
            return new CoinbaseApiAuthenticator(apiKey, apiSecret, useTimeApi, JsonSettings);
        }

        /// <summary>
        /// Creates a new merchant order checkout product.
        /// All checkouts and subsequent orders created using this endpoint are created for merchant’s primary account.
        /// Using this endpoint to create checkouts and orders is useful when you want to build a merchant checkout experience with Coinbase’s merchant tools.
        /// </summary>
        public virtual CoinbaseResponse CreateCheckout(CheckoutRequest checkout)
        {
            return SendRequest("checkouts", checkout);
        }

        /// <summary>
        /// Get the API server time.
        /// </summary>
        public virtual Time GetTime()
        {
            var resp = SendRequest<Time>("time", null, Method.GET);
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
            if (string.IsNullOrWhiteSpace(id))
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