using Datapack.Net.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Amethyst.PM
{
    [JsonConverter(typeof(DependencyHandlerJsonConverter))]
    public class DependencyHandler
    {
        internal readonly List<DependencyEntry> Entries = [];

        public DependencyEntry this[string name] => Entries.First(x => x.Name == name);

        public void Set(DependencyEntry entry)
        {
            for (var i = 0; i < Entries.Count; i++)
            {
                if (Entries[i].Name == entry.Name)
                {
                    Entries[i] = entry;
                    return;
                }
            }

            Entries.Add(entry);
        }

        // Kind of defeats the purpose of inheritance, but I don't really care right now
        public IEnumerable<DependencyResult> ResolveDependencies(ProjectDefinition project) =>
            [.. Entries.SelectMany(i => i.Resolve(project)), .. Cluster.Resolve(Entries.OfType<PackageEntry>(), project.PackFormat).SelectMany(i => i.Resolve(project))];

        public Dictionary<string, MinecraftVersion> GetDynamicClusterDependencies() =>
            new(Entries.OfType<PackageEntry>().Where(i => i.Type == LinkType.Dynamic).Select(i => new KeyValuePair<string, MinecraftVersion>(i.Name, i.Version)));
    }

    public class DependencyHandlerJsonConverter : JsonConverter<DependencyHandler>
    {
        public override void WriteJson(JsonWriter writer, DependencyHandler? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value is null ? null : new JObject(value.Entries.Select(i => i.Serialize())));
        }

        public override DependencyHandler ReadJson(JsonReader reader, Type objectType, DependencyHandler? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            existingValue ??= new();
            var obj = JObject.Load(reader);

            foreach (var (name, entry) in obj)
            {
                existingValue.Set(DependencyEntry.Parse(name, entry ?? throw new FormatException($"Empty token for entry \"{name}\"")));
            }

            return existingValue;
        }
    }
}