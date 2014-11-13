using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coinbase.ObjectModel
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ButtonStyle
    {
        // Setting a non-zero value that is not default(int) corrects
        // JSON.NET serialization output when DefaultValueHandleing.Ignore
        // should not have any effect since int values are not used in
        // coinbase's API.
        [EnumMember(Value = "buy_now_large")]
        BuyNowLarge = 1,
        [EnumMember(Value = "buy_now_small")]
        BuyNowSmall,
        [EnumMember(Value = "donation_large")]
        DonationLarge,
        [EnumMember(Value = "donation_small")]
        DonationSmall,
        [EnumMember(Value = "subscription_large")]
        SubscriptionLarge,
        [EnumMember(Value = "subscription_small")]
        SubscriptionSmall,
        [EnumMember(Value = "custom_large")]
        CustomLarge,
        [EnumMember(Value = "custom_small")]
        CustomSmall,
        None
    }

}