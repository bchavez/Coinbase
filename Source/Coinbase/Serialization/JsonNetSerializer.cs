using Newtonsoft.Json;
using RestSharp.Serializers;

namespace Coinbase.Serialization
{
    public class JsonNetSerializer : ISerializer
    {
        private readonly JsonSerializerSettings settings;

        /// <summary>
        /// Default serializer
        /// </summary>
        public JsonNetSerializer()
        {
        }

        /// <summary>
        /// Default serializer with overload for allowing custom Json.NET settings
        /// </summary>
        public JsonNetSerializer( JsonSerializerSettings settings )
        {
            this.settings = settings;
        }

        /// <summary>
        /// Serialize the object as JSON
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON as String</returns>
        public string Serialize( object obj )
        {
            return JsonConvert.SerializeObject( obj, settings );
        }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string RootElement { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public string ContentType {
            get
            {
                return "application/json";
            }
            set{}
        }
    }

}