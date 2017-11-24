using System;
using Newtonsoft.Json;

namespace Coinbase.Converters
{
   public class DecimalStringConverter : JsonConverter
   {
      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         if( value == null )
            writer.WriteNull();

         writer.WriteValue(value.ToString());
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         if( objectType == typeof(decimal?) && reader.TokenType == JsonToken.Null )
         {
            return null;
         }
         return decimal.Parse(reader.Value as string);
      }

      public override bool CanConvert(Type objectType)
      {
         return objectType == typeof(decimal) || objectType == typeof(decimal?);
      }
   }
}