using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Transporter.Core.Utils
{
    public static class ObjectExtensions
    {
        private static JsonSerializerSettings JsonSerializerSettings { get; } = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            Converters = new List<JsonConverter> {new StringEnumConverter {CamelCaseText = true}}
        };

        public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj, JsonSerializerSettings);

        public static T ToObject<T>(this string serializedJson) => JsonConvert.DeserializeObject<T>(serializedJson, JsonSerializerSettings);
    }
}