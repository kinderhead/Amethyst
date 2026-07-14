using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Amethyst.Daemon
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    [JsonObject(MemberSerialization.OptIn)]
    public class Config
    {
        [JsonProperty(PropertyName = "version")]
        public Version AmethystVersion = new();

        [JsonProperty(PropertyName = "java")]
        public string Java = "";

        [JsonProperty(PropertyName = "memory")]
        public string Memory = "";

        [JsonProperty(PropertyName = "timeout")]
        public float Timeout;
    }
}