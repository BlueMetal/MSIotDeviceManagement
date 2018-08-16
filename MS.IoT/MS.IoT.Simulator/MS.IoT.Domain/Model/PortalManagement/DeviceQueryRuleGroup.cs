using Newtonsoft.Json;
using System.Collections.Generic;

namespace MS.IoT.Domain.Model
{
    public class DeviceQueryRuleGroup
    {
        [JsonProperty(PropertyName = "operator")]
        public LogicalOperators Operator { get; set; }

        [JsonProperty(PropertyName = "rules")]
        public List<DeviceQueryRule> Rules { get; set; }

        [JsonProperty(PropertyName = "groups")]
        public List<DeviceQueryRuleGroup> Groups { get; set; }

        [JsonProperty(PropertyName = "depth")]
        public int Depth { get; set; }

        public DeviceQueryRuleGroup()
        {
            Rules = new List<DeviceQueryRule>();
            Groups = new List<DeviceQueryRuleGroup>();
        }

        public DeviceQueryRuleGroup(LogicalOperators logicalOperator)
        {
            Operator = logicalOperator;
            Rules = new List<DeviceQueryRule>();
            Groups = new List<DeviceQueryRuleGroup>();
        }
    }
}
