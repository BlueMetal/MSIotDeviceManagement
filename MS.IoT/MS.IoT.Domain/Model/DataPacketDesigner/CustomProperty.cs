using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MS.IoT.Domain.Model
{
    public enum CustomPropertyType
    {
        [EnumMember(Value = "text")]
        Text = 0,
        [EnumMember(Value = "boolean")]
        Boolean = 1,
        [EnumMember(Value = "number")]
        Number = 2,
        [EnumMember(Value = "date")]
        Date = 3,
        [EnumMember(Value = "list")]
        List = 4,
        [EnumMember(Value = "object")]
        Object = 5
    }

    public class CustomProperty
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public CustomPropertyType Type { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public List<CustomProperty> Properties { get; set; }
    }
}
