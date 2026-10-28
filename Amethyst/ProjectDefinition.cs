using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Amethyst.PM;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Newtonsoft.Json;
using Spectre.Console;

namespace Amethyst
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties)]
    [JsonObject(MemberSerialization.OptIn)]
    public partial class ProjectDefinition
    {
        private static readonly JsonSerializerSettings jsonSettings = new()
        {
            Formatting = Formatting.Indented, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
        };

        [JsonProperty("data", Order = 2)]
        [DefaultValue("data")]
        public string DataDir = "data";

        [JsonProperty("dependencies", Order = 3)]
        public DependencyHandler Dependencies = new();

        [JsonProperty("description", Order = -3)]
        public required string Description;

        [JsonProperty("name", Order = -4)]
        public required string Name;

        [JsonProperty("pack_format", Order = -2)]
        public PackFormat PackFormat;

        [JsonProperty("source", Order = 1)]
        [DefaultValue("src")]
        public string SourceDir = "src";

        [JsonProperty("version", Order = -1)]
        public MinecraftVersion Version = new(1, 0, 0);

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "Should be fine")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "Should be fine")]
        public string Serialize()
        {
            using var text = new StringWriter();
            using var writer = new JsonTextWriter(text);
            writer.Indentation = 4;

            JsonSerializer.CreateDefault(jsonSettings).Serialize(writer, this);
            return text.ToString();
        }

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "Should be fine")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "Should be fine")]
        public static ProjectDefinition Deserialize(string path)
        {
            var project = JsonConvert.DeserializeObject<ProjectDefinition>(File.ReadAllText(path), jsonSettings) ?? throw new FormatException("Could not deserialize project");

            if (!ValidNameRegex().IsMatch(project.Name))
                throw new FormatException($"{project.Name} is not a valid package name. Only lowercase alphanumeric characters, :, /, -, and _ are allowed.");

            return project;
        }

        public static bool Deserialize(string path, out ProjectDefinition project, bool showError = true)
        {
            try
            {
                project = Deserialize(path);
                return true;
            }
            catch (Exception e)
            {
                if (showError) AnsiConsole.MarkupLineInterpolated($"[red]Error parsing {path}:[/] [yellow]{e.Message}[/]");
            }

            project = null!;
            return false;
        }

        [GeneratedRegex(@"^[a-z0-9\-_:\/]+$")]
        private static partial Regex ValidNameRegex();
    }
}