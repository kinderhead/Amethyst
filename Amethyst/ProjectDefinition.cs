using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Amethyst
{
	[JsonObject(MemberSerialization.OptIn)]
	public partial struct ProjectDefinition()
	{
		[JsonProperty("name")] public string Name;

		[JsonProperty("description")] public string Description;

		[JsonProperty("version")] public MinecraftVersion Version = new(1, 0, 0);

		[JsonProperty("pack_format")] public PackFormat PackFormat;

		[JsonProperty("dependencies")] public SortedDictionary<string, MinecraftVersion> Dependencies = [];

		[JsonProperty("source")] [DefaultValue("src")]
		public string SourceDir = "src";

		[JsonProperty("data")] [DefaultValue("data")]
		public string DataDir = "data";

		public readonly string Serialize()
		{
			using var text = new StringWriter();

			using var writer = new JsonTextWriter(text);
			writer.Indentation = 4;

			JsonSerializer.CreateDefault(JsonSettings).Serialize(writer, this);
			return JsonConvert.SerializeObject(this, JsonSettings);
		}

		public static ProjectDefinition Deserialize(string path)
		{
			var project = JsonConvert.DeserializeObject<ProjectDefinition>(File.ReadAllText(path), JsonSettings);

			if (!ValidNameRegex().IsMatch(project.Name))
			{
				throw new FormatException(
					$"{project.Name} is not a valid package name. Only lowercase alphanumeric characters, -, and _ are allowed.");
			}

			return project;
		}

		[GeneratedRegex(@"^[a-z0-9\-_]+$")]
		private static partial Regex ValidNameRegex();

		private static readonly JsonSerializerSettings JsonSettings = new()
		{
			Formatting = Formatting.Indented, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
		};
	}
}