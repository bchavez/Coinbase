using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coinbase.ObjectModel
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ButtonType
    {
        // Setting a non-zero value that is not default(int) corrects
        // JSON.NET serialization output when DefaultValueHandleing.Ignore
        // should not have any effect since int values are not used in
        // coinbase's API.
        [EnumMember(Value = "buy_now")]
        BuyNow = 1,
        Donation,
    }
}