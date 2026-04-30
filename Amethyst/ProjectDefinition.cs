using Datapack.Net.Pack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace Amethyst
{
    [JsonObject(MemberSerialization.OptIn)]
    public readonly struct ProjectDefinition()
    {
        [JsonProperty("name")]
        public readonly string Name { get; init; }

        [JsonProperty("description")]
        public readonly string Description { get; init; }

        [JsonProperty("version")]
        [JsonConverter(typeof(SemVerJsonConverter))]
        public readonly SemVer Version { get; init; } = SemVer.Create(1, 0, 0);

        [JsonProperty("pack_version")]
        public readonly PackVersion PackVersion { get; init; } = PackVersion.Latest;

        [JsonProperty("source")]
        [DefaultValue("src")]
        public readonly string SourceDir { get; init; } = "src";

        [JsonProperty("data")]
        [DefaultValue("data")]
        public readonly string DataDir { get; init; } = "data";

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }
    }

    public class SemVerJsonConverter : JsonConverter<SemVer>
    {
        public override SemVer ReadJson(JsonReader reader, Type objectType, SemVer existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var data = JToken.Load(reader);
            return SemVer.Parse((string?)data ?? throw new FormatException("Expected string for SemVer."));
        }

        public override void WriteJson(JsonWriter writer, SemVer value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }
    }
}
