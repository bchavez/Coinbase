using System;
using System.Configuration;
using System.Net;
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
    public class CoinbaseApi
    {
        private readonly string apiKey;

        private JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        public const string BaseUrl = "https://coinbase.com/api/v1/{action}?api_key={apikey}";
        public const string CheckoutPageUrl = "https://coinbase.com/checkouts/{code}";

        public CoinbaseApi() : this(string.Empty)
        {
        }

        public CoinbaseApi( string apiKey )
        {
            this.apiKey = !string.IsNullOrWhiteSpace( apiKey ) ? apiKey : ConfigurationManager.AppSettings["CoinbaseApiKey"];
            if ( string.IsNullOrWhiteSpace( this.apiKey ) )
            {
                throw new ArgumentException( "The API key must not be empty. A valid API key should be used in the CoinbaseApi constructor or an appSettings configuration element with <add key='CoinbaseApiKey' value='my_api_key' /> should exist.", "apiKey" );
            }
        }

        public CoinbaseApi(string apiKey, JsonSerializerSettings settings) : this(apiKey)
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
            client.AddHandler( "application/json", new JsonNetDeseralizer( settings ) );
            return client;
        }

        protected virtual IRestRequest CreateRequest( string action )
        {
            var post = new RestRequest( action, Method.POST )
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = new JsonNetSerializer(settings),
                    
                }
                .AddParameter( "api_key", this.apiKey, ParameterType.QueryString );
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
        New
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
