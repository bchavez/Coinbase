using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coinbase.ObjectModel
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscriptionType
    {
        Never = 1,
        Hourly,
        Daily,
        Weekly,
        [EnumMember(Value = "every_two_weeks")]
        EveryTwoWeeks,
        Monthly,
        Quarterly,
        Yearly
    }
}