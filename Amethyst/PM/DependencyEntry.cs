using System.Diagnostics.CodeAnalysis;
using Datapack.Net.Utils;
using Newtonsoft.Json.Linq;

namespace Amethyst.PM
{
    public readonly record struct DependencyResult(string ZipPath, LinkType Type);

    public abstract class DependencyEntry(string name, LinkType type)
    {
        public readonly string Name = name;
        public readonly LinkType Type = type;

        public abstract IEnumerable<DependencyResult> Resolve(ProjectDefinition project);

        public JProperty Serialize()
        {
            if (Type == LinkType.Dynamic)
            {
                return
                [
                    with(Name, new JObject
                    {
                        ["type"] = "dynamic",
                        ["version"] = SerializeVersion()
                    })
                ];
            }

            return [with(Name, SerializeVersion())];
        }

        protected abstract string SerializeVersion();

        public static DependencyEntry Parse(string name, JToken entry)
        {
            var type = LinkType.Static;
            var token = entry;

            if (entry is JObject obj)
            {
                if (obj.TryGetValue("link", out var value))
                {
                    type = (string?)value switch
                    {
                        "dynamic" => LinkType.Dynamic,
                        "static" => LinkType.Static,
                        _ => throw new FormatException($"Invalid link type {(string?)value} for dependency \"{name}\"")
                    };

                    if (!obj.TryGetValue("version", out token)) throw new FormatException($"Missing field \"version\" for dependency \"{name}\"");
                }
            }

            if (PackageEntry.Parse(name, type, token) is { } ret1) return ret1;
            if (FileEntry.Parse(name, type, token) is { } ret2) return ret2;

            throw new FormatException($"Invalid dependency \"{name}\" with contents {token}");
        }
    }

    public class PackageEntry(string name, LinkType type, MinecraftVersion version) : DependencyEntry(name, type)
    {
        public readonly MinecraftVersion Version = version;

        public override IEnumerable<DependencyResult> Resolve(ProjectDefinition project) => [];
        protected override string SerializeVersion() => Version.ToString();

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "<Pending>")]
        public static PackageEntry? Parse(string name, LinkType type, JToken token)
        {
            try
            {
                return new(name, type, token.ToObject<MinecraftVersion>());
            }
            catch
            {
                return null;
            }
        }
    }

    public class FileEntry(string name, LinkType type, string path) : DependencyEntry(name, type)
    {
        public readonly string Path = path;

        public override IEnumerable<DependencyResult> Resolve(ProjectDefinition project) => [new(Path, Type)];
        protected override string SerializeVersion() => $"file:{Path}";

        public static FileEntry? Parse(string name, LinkType type, JToken token)
        {
            try
            {
                var path = (string?)token;
                if (path is null || !path.StartsWith("file:")) return null;
                return new(name, type, path[5..]);
            }
            catch
            {
                return null;
            }
        }
    }
}