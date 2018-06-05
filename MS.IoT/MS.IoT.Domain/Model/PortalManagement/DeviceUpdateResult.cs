using Newtonsoft.Json;

namespace MS.IoT.Domain.Model
{
    public class DeviceUpdateResult
    {
        [JsonProperty(PropertyName = "hasSucceeded")]
        public bool HasSucceeded { get; set; }

        [JsonProperty(PropertyName = "errorReason")]
        public string ErrorReason { get; set; }

        [JsonProperty(PropertyName = "jobId")]
        public string JobId { get; set; }

        public DeviceUpdateResult() { }

        public DeviceUpdateResult(bool success, string jobId, string errorReason)
        {
            HasSucceeded = success;
            ErrorReason = errorReason;
            JobId = jobId;
        }
    }
}
