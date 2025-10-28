using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Amethyst.Daemon
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    [JsonObject(MemberSerialization.OptIn)]
    public class Config
    {
        [JsonProperty(PropertyName = "version")]
        public Version AmethystVersion = new();

        [JsonProperty(PropertyName = "memory")]
        public string Memory = "";

        [JsonProperty(PropertyName = "java")]
        public string Java = "";

        [JsonProperty(PropertyName = "timeout")]
        public float Timeout = 0;
    }
}
