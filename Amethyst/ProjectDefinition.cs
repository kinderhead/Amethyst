using Datapack.Net.Pack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Amethyst
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial struct ProjectDefinition()
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("version")]
        public SemVer Version = SemVer.Create(1, 0, 0);

        [JsonProperty("pack_version")]
        public PackVersion PackVersion;

        [JsonProperty("dependencies")]
        public SortedDictionary<string, SemVer> Dependencies = [];

        [JsonProperty("source")]
        [DefaultValue("src")]
        public string SourceDir = "src";

        [JsonProperty("data")]
        [DefaultValue("data")]
        public string DataDir = "data";

        public readonly string Serialize()
        {
            return JsonConvert.SerializeObject(this, JsonSettings);
        }

        public static ProjectDefinition Deserialize(string path)
        {
            var project = JsonConvert.DeserializeObject<ProjectDefinition>(File.ReadAllText(path), JsonSettings);

            if (!ValidNameRegex().IsMatch(project.Name))
            {
                throw new FormatException($"{project.Name} is not a valid package name. Only lowercase alphanumeric characters, -, and _ are allowed.");
            }

            return project;
        }

        [GeneratedRegex(@"^[a-z0-9\-_]+$")]
        private static partial Regex ValidNameRegex();

        private static readonly JsonSerializerSettings JsonSettings = new()
		{
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Converters = [new SemVerJsonConverter()]
        };
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
