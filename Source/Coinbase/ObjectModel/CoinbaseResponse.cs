using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Coinbase.ObjectModel
{
   public class CoinbaseResponse<T>
   {
      public Pagination Pagination { get; set; }

      public T Data { get; set; }

      /// <summary>
      /// Error responses described here:
      /// https://developers.coinbase.com/api/v2?shell#error-response
      /// </summary>
      public ErrorOrWarning[] Errors { get; set; }

      /// <summary>
      /// Warning responses described here:
      /// https://developers.coinbase.com/api/v2?shell#error-response
      /// </summary>
      public ErrorOrWarning[] Warnings { get; set; }

      [JsonExtensionData]
      public IDictionary<string, JToken> ExtraData { get; set; }
   }

   public class CoinbaseResponse : CoinbaseResponse<JToken>
   {
   }

   public class Notification : CoinbaseResponse
   {
      [JsonIgnore]
      public bool IsVerified => false;

      /// <summary>
      /// You must first verify the order before this property becomes available.
      /// Verify the notification by calling CoinbaseApi.VerifyNotification() with the Notification and the associated "X-Signature" (or CB-SIGNATURE) request header value.
      /// </summary>
      //[JsonIgnore]
      //internal JObject Order
      //{
      //    get
      //    {
      //        if( !IsVerified )
      //        {
      //            throw new ArgumentNullException(nameof(Order),
      //                "The coinbase Notification callback has not been verified. " +
      //                "You must first verify this notification before " +
      //                $"the {nameof(Order)} property becomes available. " +
      //                "Verify the notification by calling CoinbaseApi.VerifyNotification() " +
      //                "with the Notification and the associated 'X-Signature' (or 'CB-SIGNATURE')" +
      //                " HTTP request header value that coinbase has supplied." 
      //                );
      //        }
      //        return UnverifiedOrder;
      //    }
      //}

      [JsonProperty("order")]
      public JObject UnverifiedOrder { get; set; }

      [JsonExtensionData]
      public IDictionary<string, JToken> ExtraData { get; set; }
   }

   public class ErrorOrWarning
   {
      public string Id { get; set; }
      public string Message { get; set; }
      public string Url { get; set; }

      [JsonExtensionData]
      public IDictionary<string, JToken> ExtraData { get; set; }
   }

   public class Pagination
   {
      [JsonProperty("ending_before")]
      public string EndingBefore { get; set; }

      [JsonProperty("starting_after")]
      public string StartingAfter { get; set; }

      public int Limit { get; set; }

      [JsonConverter(typeof(StringEnumConverter))]
      public SortOrder Order { get; set; }

      [JsonProperty("previous_uri")]
      public object PreviousUri { get; set; }

      [JsonProperty("next_uri")]
      public string NextUri { get; set; }
   }


   public enum SortOrder
   {
      Desc,
      Asc
   }

   public class CheckoutRequest
   {
      public CheckoutRequest()
      {
         this.Metadata = new Dictionary<string, object>();
      }

      public decimal Amount { get; set; }
      public string Currency { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }

      [JsonConverter(typeof(StringEnumConverter))]
      public OrderType Type { get; set; }

      public string Style { get; set; }

      [JsonProperty("customer_defined_amount")]
      public bool? CustomerDefinedAmount { get; set; }

      [JsonProperty("amount_presets")]
      public decimal[] AmountPresets { get; set; }

      [JsonProperty("success_url")]
      public string SuccessUrl { get; set; }

      [JsonProperty("cancel_url")]
      public string CancelUrl { get; set; }

      [JsonProperty("notifications_url")]
      public string NotificationsUrl { get; set; }

      [JsonProperty("auto_redirect")]
      public bool? AutoRedirect { get; set; }

      [JsonProperty("collect_shipping_address")]
      public bool? CollectShippingAddress { get; set; }

      [JsonProperty("collect_email")]
      public bool? CollectEmail { get; set; }

      [JsonProperty("collect_phone_number")]
      public bool? CollectPhoneNumber { get; set; }

      [JsonProperty("collect_country")]
      public bool? CollectCountry { get; set; }

      public Dictionary<string, object> Metadata { get; set; }
   }

   public enum OrderType
   {
      Order,
      Donation
   }

   public enum OrderStyle
   {
      buy_now_large,
      buy_now_small,
      donation_large,
      donation_small,
      custom_large,
      custom_small
   }

   public class Time
   {
      public DateTimeOffset Iso { get; set; }
      public ulong Epoch { get; set; }
   }
}