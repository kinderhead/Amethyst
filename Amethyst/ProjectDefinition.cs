using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Newtonsoft.Json;

namespace Amethyst
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    [JsonObject(MemberSerialization.OptIn)]
    public partial struct ProjectDefinition()
    {
        [JsonProperty("name")]
        public required string Name;

        [JsonProperty("description")]
        public required string Description;

        [JsonProperty("version")]
        public MinecraftVersion Version = new(1, 0, 0);

        [JsonProperty("pack_format")]
        public PackFormat PackFormat;

        [JsonProperty("dependencies")]
        public SortedDictionary<string, MinecraftVersion> Dependencies = [];

        [JsonProperty("source")]
        [DefaultValue("src")]
        public string SourceDir = "src";

        [JsonProperty("data")]
        [DefaultValue("data")]
        public string DataDir = "data";

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "Should be fine")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "Should be fine")]
        public readonly string Serialize()
        {
            using var text = new StringWriter();

            using var writer = new JsonTextWriter(text);
            writer.Indentation = 4;

            JsonSerializer.CreateDefault(jsonSettings).Serialize(writer, this);
            return JsonConvert.SerializeObject(this, jsonSettings);
        }

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "Should be fine")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "Should be fine")]
        public static ProjectDefinition Deserialize(string path)
        {
            var project = JsonConvert.DeserializeObject<ProjectDefinition>(File.ReadAllText(path), jsonSettings);

            if (!ValidNameRegex().IsMatch(project.Name))
            {
                throw new FormatException(
                    $"{project.Name} is not a valid package name. Only lowercase alphanumeric characters, -, and _ are allowed.");
            }

            return project;
        }

        [GeneratedRegex(@"^[a-z0-9\-_]+$")]
        private static partial Regex ValidNameRegex();

        private static readonly JsonSerializerSettings jsonSettings = new()
        {
            Formatting = Formatting.Indented, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
        };
    }
}