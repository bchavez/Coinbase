using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Coinbase.Models
{
   public class Json
   {
      /// <summary>
      /// Extra data for/from the JSON serializer/deserializer to included with the object model.
      /// </summary>
      [JsonExtensionData]
      public IDictionary<string, JToken> ExtraJson { get; internal set; } = new Dictionary<string, JToken>();
   }

   public class JsonResponse : Json
   {
      /// <summary>
      /// All error messages include a type identifier and a human readable message.
      /// </summary>
      public Error[] Errors { get; set; }

      /// <summary>
      /// Responses can include a warnings parameter to notify the developer
      /// of best practices, implementation suggestions or deprecation warnings.
      /// While you don’t need show warnings to the user, they are usually
      /// something you need to act on.
      /// </summary>
      public Warning[] Warnings { get; set; }

      /// <summary>
      /// Checks if the response has errors.
      /// </summary>
      public bool HasError()
      {
         return this.Errors?.Length > 0;
      }

      /// <summary>
      /// Checks if the response has warnings.
      /// </summary>
      public bool HasWarnings()
      {
         return this.Warnings?.Length > 0;
      }
   }

   public class Response<T> : JsonResponse
   {
      [JsonProperty("data")]
      public T Data { get; set; }
   }

   public class PagedResponse<T> : JsonResponse
   {
      [JsonProperty("pagination")]
      public Pagination Pagination { get; set; }

      [JsonProperty("data")]
      public T[] Data { get; set; }
   }

   public class Error : Json
   {
      public string Id { get; set; }
      public string Message { get; set; }
      public string Url { get; set; }
   }
   public class Warning : Json
   {
      public string Id { get; set; }
      public string Message { get; set; }
      public string Url { get; set; }
   }

   public class Pagination : Json
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

}