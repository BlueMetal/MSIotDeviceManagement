using Newtonsoft.Json;

namespace MS.IoT.Domain.Model
{
    public class DeviceFieldModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public FieldTypes Type { get; set; }

        public DeviceFieldModel()
        {

        }

        public DeviceFieldModel(string name, FieldTypes type)
        {
            Name = name;
            Type = type;
        }
    }
}
