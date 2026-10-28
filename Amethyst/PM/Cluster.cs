using System.Diagnostics.CodeAnalysis;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace Amethyst.PM
{
    public class Cluster
    {
        private static readonly HttpClient client = new();
        private static readonly Dictionary<string, JObject> downloadCache = [];

        public readonly string Location;
        private readonly JObject data;

        private Cluster(JObject data, string location)
        {
            Location = location;
            this.data = data;
        }

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "<Pending>")]
        public void Add(string version, PackFormat format, string link, Dictionary<string, string> dependencies)
        {
            data.Merge(new JObject
            {
                ["@versions"] = new JObject
                {
                    [version] = new JObject
                    {
                        ["pack_format"] = format.ToString(),
                        ["link"] = link,
                        ["dependencies"] = JObject.FromObject(dependencies)
                    }
                }
            });
        }

        public void Save()
        {
            File.WriteAllText(Location, Serialize());
        }

        public string ResolvePath(string relativePath)
        {
            if (Path.IsPathFullyQualified(relativePath)) return relativePath;
            if (Uri.TryCreate(relativePath, UriKind.Absolute, out var res)) return res.ToString();
            if (Uri.TryCreate(Location, UriKind.Absolute, out var uri) && uri.Scheme != Uri.UriSchemeFile)
                return new Uri(uri, relativePath.StartsWith('.') ? relativePath : $"./{relativePath}").ToString();
            return Path.GetRelativePath(Environment.CurrentDirectory, Path.GetFullPath(Path.Join(Path.GetDirectoryName(Location), relativePath)));
        }

        public string Serialize()
        {
            using var text = new StringWriter();
            using var writer = new JsonTextWriter(text);
            writer.Indentation = 4;
            writer.Formatting = Formatting.Indented;
            data.WriteTo(writer);
            return text.ToString();
        }

        public IEnumerable<MinecraftVersion> GetVersions(string id) => Lookup(id).Properties().Select(i => new MinecraftVersion(i.Name));

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "<Pending>")]
        public ClusterEntry Query(string id, string version)
        {
            var obj = Lookup(id);
            if (!(obj.TryGetValue(version, out var e) && e is JObject entry)) throw new KeyNotFoundException($"{id} does not have version {version}");
            if (!(entry.TryGetValue("pack_format", out var format) && format.Type == JTokenType.String))
                throw new FormatException($"{id}@{version} missing pack_format");
            if (!(entry.TryGetValue("link", out var link) && format.Type == JTokenType.String))
                throw new FormatException($"{id}@{version} missing link");

            var dependencies = new Dictionary<string, string>();
            if (entry.TryGetValue("dependencies", out var deps))
                dependencies = deps.ToObject<Dictionary<string, string>>() ?? throw new FormatException($"{id}@{version} has malformed dependencies");

            return new(new((string?)format!), (string?)link!, dependencies);
        }

        public ClusterEntry? TryQuery(string id, string version)
        {
            // Tee hee
            try
            {
                return Query(id, version);
            }
            catch
            {
                return null;
            }
        }

        private JObject Lookup(string id)
        {
            var current = data;

            foreach (var i in id.Contains(':') ? new NamespacedID(id).Parts : [id])
            {
                if (current.TryGetValue(i, out var tok))
                {
                    if (tok is JObject obj) current = obj;
                    if (tok.Type == JTokenType.String) current = GlobalFetchJson(ResolvePath((string?)tok ?? throw new FormatException($"Token {tok} is malformed")));
                    continue;
                }

                throw new KeyNotFoundException($"{id} does not exist");
            }

            if (!(current.TryGetValue("@versions", out var versions) && versions is JObject { Count: > 0 } ret))
                throw new KeyNotFoundException($"{id} exists but does not expose versions");

            return ret;
        }

        public static JObject GlobalFetchJson(string path)
        {
            if (downloadCache.TryGetValue(path, out var data)) return data;

            using var file = GlobalFetch(path);
            using var reader = new StreamReader(file);
            using var json = new JsonTextReader(reader);
            var res = (JObject)JToken.ReadFrom(json);
            downloadCache[path] = res;
            return res;
        }

        public static Stream GlobalFetch(string path) => TryGlobalFetch(path) ?? throw new FileNotFoundException($"Could not load cluster file: {path}", path);

        public static Stream? TryGlobalFetch(string path)
        {
            if (File.Exists(path)) return File.OpenRead(path);

            try
            {
                return client.GetStreamAsync(path).GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }

        public static Cluster From(string path) => new(GlobalFetchJson(path), path);
        public static Cluster From(string path, JObject data) => new(data, path);
        public static Cluster New(string path) => new([], path);

        public static T? WithCli<T>(Func<T> cb)
        {
            T? ret = default;

            try
            {
                ret = cb();
            }
            catch (Exception e)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Cluster error:[/] [yellow]{e.Message}[/]");
            }

            return ret;
        }

        public static int WithCli(Action cb) => WithCli(() =>
        {
            cb();
            return new object();
        }) is null
            ? 1
            : 0;

        public static void AddDownloadCacheEntry(string name, JObject obj) => downloadCache[name] = obj;
    }

    public readonly record struct ClusterEntry(PackFormat Format, string Link, Dictionary<string, string> Dependencies);
}