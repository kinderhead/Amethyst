using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Amethyst
{
    [JsonObject(MemberSerialization.OptIn)]
    public readonly struct ProjectDefinition
    {
        [JsonProperty("name")]
        public readonly string Name;

        [JsonProperty("description")]
        [DefaultValue("A project made with Amethyst")]
        public readonly string Description;

        [JsonProperty("version")]
        public readonly SemVer Version;

        [JsonProperty("src")]
        [DefaultValue("src")]
        public readonly string SourceDir;

        [JsonProperty("data")]
        [DefaultValue("data")]
        public readonly string DataDir;

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }
    }
}
