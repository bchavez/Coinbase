using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Coinbase.Converters;
using Coinbase.Serialization;
using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using RestSharp;

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

            var body = request.Parameters.First(p => p.Type == ParameterType.RequestBody).Value;

            var hmacSig = GenerateSignature(nonce, url.ToString(), body.ToString(), this.apiSecret);

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
            var hmacKey = Encoding.ASCII.GetBytes( key );

            using( var signatureStream = new MemoryStream( Encoding.ASCII.GetBytes( data ) ) )
            {
                var hex = new HMACSHA256( hmacKey ).ComputeHash( signatureStream )
                    .Aggregate( new StringBuilder(), ( sb, b ) => sb.AppendFormat( "{0:x2}", b ), sb => sb.ToString() );

                return hex;
            }
        }
    }

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

    public class OrderResponse : CoinbaseResponse
    {
        public Order Order { get; set; }
    }

    public class Order
    {
        public string Id { get; set; }
        [JsonProperty("created_at")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? CreatedAt { get; set; }

        public Status Status { get; set; }

        [JsonProperty("total_btc")]
        public Price TotalBtc{get; set;}

        [JsonProperty("total_native")]
        public Price TotalNative { get; set; }

        public string Custom { get; set; }

        [JsonProperty( "receive_address" )]
        public string ReceiveAddress { get; set; }

        public ButtonDesc Button { get; set; }

        public Transaction Transaction { get; set; }

        public Customer Customer { get; set; }
    }

    public class ButtonDesc
    {
        public ButtonType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
    }

    public enum Status
    {
        Canceled,
        Completed = 0x01,
        New,
        Mispaid
    }

    public class Customer
    {
        public string Email { get; set; }

        [JsonProperty( "shipping_address" )]
        public string[] ShippingAddress { get; set; }
    }

    public class Transaction
    {
        public string Id { get; set; }
        public string Hash { get; set; }
        public int Confirmations { get; set; }
    }

    public class ButtonResponse : CoinbaseResponse
    {
        public ButtonCreated Button { get; set; }

        public string GetCheckoutUrl()
        {
            var url = CoinbaseApi.CheckoutPageUrl
                .Replace( "{code}", Button.Code );

            return url;
        }
    }

    public abstract class CoinbaseResponse
    {
        public bool Success { get; set; }

        public string[] Errors { get; set; }
    }

    public class ButtonCreated : ButtonRequest
    {
        public string Code { get; set; }
        public new Price Price { get; set; }

        public new Currency Currency
        {
            get
            {
                return this.Price.Currency;
            }
        }
    }

    public class Price
    {
        public int Cents { get; set; }

        [JsonProperty( "currency_iso" )]
        public Currency Currency { get; set; }
    }

    /// <summary>
    /// Authenticated resource that creates a payment button, page, or iFrame to accept bitcoin on your website. This can be used to accept bitcoin for an individual item or to integrate with your existing shopping cart solution. For example, you could create a new payment button for each shopping cart on your website, setting the total and order number in the button at checkout.
    ///
    /// The code param returned in the response can be used to generate an embeddable button, payment page, or iFrame.
    /// </summary>
    [Validator(typeof(ButtonValidator))]
    public class ButtonRequest
    {
        /// <summary>
        /// The name of the item for which you are collecting bitcoin. For example, Acme Order #123 or Annual Pledge Drive
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// One of buy_now, donation, and subscription. Default is buy_now.
        /// </summary>
        public ButtonType Type { get; set; }
        
        /// <summary>
        /// Price as a decimal string, for example 1.23. Can be more then two significant digits if price_currency_iso equals BTC.
        /// </summary>
        [JsonProperty("price_string")]
        [JsonConverter(typeof(DecimalStringConverter))]
        public decimal Price { get; set; }

        /// <summary>
        /// Price currency as an ISO 4217 code such as USD or BTC. This determines what currency the price is shown in on the payment widget.
        /// </summary>
        [JsonProperty( "price_currency_iso" )]     
        public Currency Currency { get; set; }

        /// <summary>
        /// An optional custom parameter. Usually an Order, User, or Product ID corresponding to a record in your database.
        /// </summary>
        public string Custom { get; set; }

        /// <summary>
        /// A custom callback URL specific to this button. It will receive the same information that would otherwise be sent to your Instant Payment Notification URL. If you have an Instant Payment Notification URL set on your account, this will be called instead — they will not both be called.
        /// </summary>
        [JsonProperty( "callback_url" )]
        public string CallbackUrl { get; set; }

        /// <summary>
        /// A custom success URL specific to this button. The user will be redirected to this URL after a successful payment. It will receive the same parameters that would otherwise be sent to the default success url set on the account.
        /// </summary>
        [JsonProperty("success_url")]
        public string SuccessUrl { get; set; }
        
        /// <summary>
        /// A custom cancel URL specific to this button. The user will be redirected to this URL after a canceled order. It will receive the same parameters that would otherwise be sent to the default cancel url set on the account.
        /// </summary>
        [JsonProperty( "cancel_url" )]
        public string CancelUrl { get; set; }

        /// <summary>
        /// Allow users to change the price on the generated button.
        /// </summary>
        [JsonProperty( "variable_price" )]
        public bool VariablePrice { get; set; }

        /// <summary>
        /// Show some suggested prices.
        /// </summary>
        [JsonProperty( "choose_price" )]
        public bool ChoosePrice { get; set; }

        /// <summary>
        /// A custom info URL specific to this button. Displayed to the user after a successful purchase for sharing. It will receive the same parameters that would otherwise be sent to the default info url set on the account.
        /// </summary>
        [JsonProperty( "info_url" )]
        public string InfoUrl { get; set; }

        /// <summary>
        /// Longer description of the item in case you want it added to the user’s transaction notes.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// One of buy_now_large, buy_now_small, donation_large, donation_small, subscription_large, subscription_small, custom_large, custom_small, and none. Default is buy_now_large. none is used if you plan on triggering the payment modal yourself using your own button or link.
        /// </summary>
        public ButtonStyle Style { get; set; }

        /// <summary>
        /// Allows you to customize the button text on custom_large and custom_small styles. Default is Pay With Bitcoin.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Collect email address from customer (not for use with inline iframes).
        /// </summary>
        [JsonProperty( "include_email" )]
        public bool IncludeEmail { get; set; }

        /// <summary>
        /// Collect shipping address from customer (not for use with inline iframes).
        /// </summary>
        [JsonProperty( "include_address" )]
        public bool IncludeAddress { get; set; }
    }

    public class ButtonValidator : AbstractValidator<ButtonRequest>
    {
        public ButtonValidator()
        {
            RuleFor( x => x.Name )
                .NotEmpty();

            RuleFor( x => x.Price )
                .NotEmpty()
                .GreaterThan( 0m );

            RuleFor( x => x.Currency )
                .Must( x => Enum.IsDefined( typeof(Currency), x ) )
                .WithMessage( "A valid currency must be used." );
        }
    }


    [JsonConverter( typeof(StringEnumConverter) )]
    public enum ButtonType
    {
        [EnumMember( Value = "buy_now" )]
        BuyNow,
        Donation,
        Subscription,
    }

    [JsonConverter( typeof( StringEnumConverter ) )]
    public enum ButtonStyle
    {
        [EnumMember( Value = "buy_now_large" )]
        BuyNowLarge,
        [EnumMember( Value = "buy_now_small" )]
        BuyNowSmall,
        [EnumMember( Value = "donation_large" )]
        DonationLarge,
        [EnumMember( Value = "donation_small" )]
        DonationSmall,
        [EnumMember( Value = "subscription_large" )]
        SubscriptionLarge,
        [EnumMember( Value = "subscription_small" )]
        SubscriptionSmall,
        [EnumMember( Value = "custom_large" )]
        CustomLarge,
        [EnumMember( Value = "custom_small" )]
        CustomSmall,
        None
    }
}
