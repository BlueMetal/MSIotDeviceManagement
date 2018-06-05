using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Model
{
    public enum TemplateDocumentType
    {
        [EnumMember(Value = "common")]
        CommonTemplate = 0,
        [EnumMember(Value = "user")]
        User = 1,
        [EnumMember(Value = "category")]
        Category = 2
    }

    public class BaseTemplateDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "docType")]
        public TemplateDocumentType DocType { get; set; }
    }
}
