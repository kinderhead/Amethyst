using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Amethyst.Daemon;
using Datapack.Net.Pack;
using Datapack.Net.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace Amethyst.PM
{
    public class Cluster
    {
        public const string ROOT_CLUSTER = "https://raw.githubusercontent.com/kinderhead/Amethyst/refs/heads/master/cluster.json";

        public static readonly string LocalClusterFolder = Path.Join(Server.AmethystFolder, "cluster");
        public static readonly string LocalCluster = Path.Join(LocalClusterFolder, "cluster.json");
        public static readonly string VersionCheck = Path.Join(LocalClusterFolder, "version.txt");

        private static readonly HttpClient client = new();
        private static readonly Dictionary<string, JObject> downloadCache = [];

        public readonly string Location;
        private readonly JObject data;

        private Cluster(JObject data, string location)
        {
            Location = location;
            this.data = data;
        }

        public void Add(string version, PackFormat format, string link, Dictionary<string, string> dependencies) => Add([], version, format, link, dependencies);

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "<Pending>")]
        public void Add(string[] path, string version, PackFormat format, string link, Dictionary<string, string> dependencies)
        {
            var start = new JObject();
            var current = start;

            foreach (var i in path)
            {
                var next = new JObject();
                current[i] = next;
                current = next;
            }

            current["@versions"] = new JObject
            {
                [version] = new JObject
                {
                    ["pack_format"] = format.ToString(),
                    ["link"] = link,
                    ["dependencies"] = JObject.FromObject(dependencies)
                }
            };

            data.Merge(start);
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

        public ClusterEntry Query(string idAndVersion)
        {
            var parts = idAndVersion.Split('@');
            return Query(parts[0], parts[1]);
        }

        public ClusterEntry Query(string id, string version)
        {
            var obj = Lookup(id);
            if (!(obj.TryGetValue(version, out var e) && e is JObject entry)) throw new KeyNotFoundException($"shard {id} does not have version {version}.");
            return JsonToEntry(entry, id, version);
        }

        public bool TryQuery(string id, string version, [NotNullWhen(true)] out ClusterEntry? entry)
        {
            // Tee hee
            try
            {
                entry = Query(id, version);
                return true;
            }
            catch
            {
                entry = null;
                return false;
            }
        }

        public ClusterEntry[] QueryAll(string id)
        {
            var shard = Lookup(id);
            var entries = new List<ClusterEntry>();

            foreach (var i in shard.Properties())
            {
                // Ignore random bad versions. Why not
                if ((JObject?)i.Value is { } obj) entries.Add(JsonToEntry(obj, id, i.Name));
            }

            entries.Sort((a, b) => a.Version.CompareTo(b.Version));

            return [.. entries];
        }

        public ClusterEntry QueryLatest(string id, PackFormat? format = null)
        {
            foreach (var i in QueryAll(id).Reverse())
            {
                if (format is null || i.Format == format) return i;
            }

            throw new InvalidOperationException($"No version of {id} supports format {format}.");
        }

        private JObject Lookup(string id)
        {
            var current = data;

            foreach (var i in id.Contains(':') ? new NamespacedID(id).Parts : [id])
            {
                if (current.TryGetValue(i, out var tok))
                {
                    if (tok is JObject obj) current = obj;
                    if (tok.Type == JTokenType.String) current = GlobalFetchJson(ResolvePath((string?)tok ?? throw new FormatException($"Token {tok} is malformed.")));
                    continue;
                }

                throw new KeyNotFoundException($"shard {id} does not exist.");
            }

            if (!(current.TryGetValue("@versions", out var versions) && versions is JObject { Count: > 0 } ret))
                throw new KeyNotFoundException($"shard {id} exists but does not expose versions.");

            return ret;
        }

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "<Pending>")]
        private static ClusterEntry JsonToEntry(JObject entry, string id, string version)
        {
            if (!(entry.TryGetValue("pack_format", out var format) && format.Type == JTokenType.String))
                throw new FormatException($"{id}@{version} missing pack_format.");
            if (!(entry.TryGetValue("link", out var link) && format.Type == JTokenType.String))
                throw new FormatException($"{id}@{version} missing link.");

            var dependencies = new Dictionary<string, string>();
            if (entry.TryGetValue("dependencies", out var deps))
                dependencies = deps.ToObject<Dictionary<string, string>>() ?? throw new FormatException($"{id}@{version} has malformed dependencies.");

            return new(id, new(version), new((string?)format!), (string?)link!, dependencies);
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

        public static Cluster GetLocal()
        {
            if (File.Exists(VersionCheck) && File.ReadAllText(VersionCheck) != Assembly.GetExecutingAssembly().GetName().Version?.ToString())
                Directory.Delete(LocalClusterFolder, true);

            Directory.CreateDirectory(Path.Join(LocalClusterFolder, "cache"));

            if (!File.Exists(VersionCheck)) File.WriteAllText(VersionCheck, Assembly.GetExecutingAssembly().GetName().Version?.ToString());

            return File.Exists(LocalCluster) ? From(LocalCluster) : New(LocalCluster);
        }

        public static IEnumerable<FileEntry> Resolve(IEnumerable<PackageEntry> dependencies, PackFormat expectedFormat)
        {
            var queue = new Queue<PackageEntry>(dependencies);
            var files = new Dictionary<string, FileEntry>();
            var toDownload = new Dictionary<string, ClusterEntry>();
            var local = GetLocal();
            var root = From(ROOT_CLUSTER);

            while (queue.TryDequeue(out var entry))
            {
                var entryKey = $"{entry.Name}@{entry.Version}";
                if (files.ContainsKey(entryKey)) continue;
                if (toDownload.ContainsKey(entryKey)) continue;

                var found = local.TryQuery(entry.Name, entry.Version.ToString(), out var l) && File.Exists(l.Value.Link)
                    ? l.Value
                    : root.Query(entry.Name, entry.Version.ToString());

                if (!File.Exists(found.Link)) toDownload[entryKey] = found;
                else
                {
                    if (found.Format != expectedFormat)
                        AnsiConsole.MarkupLineInterpolated($"[yellow]Cluster warning: {entryKey} targets pack format {found.Format} while the shard targets {expectedFormat}.[/]");

                    files[entryKey] = new(entry.Name, entry.Type, found.Link);
                    foreach (var (name, ver) in found.Dependencies)
                    {
                        queue.Enqueue(new(name, LinkType.Dynamic, new(ver)));
                    }
                }
            }

            if (toDownload.Count != 0)
            {
                if (!Server.DownloadFiles(toDownload.Select(i => (i.Key, new Uri(i.Value.Link), Path.Join(LocalClusterFolder, "cache", $"{i.Key}.zip")))))
                    Environment.Exit(1); // Probably not nice but idk

                foreach (var (name, entry) in toDownload)
                {
                    var realName = name.Split("@")[0];
                    var path = Path.Join(LocalClusterFolder, "cache", $"{name}.zip");
                    local.Add(realName.Split(':', '/'), name.Split("@")[1], entry.Format, path, entry.Dependencies);
                    files[name] = new(realName, LinkType.Dynamic, path);
                }
            }

            local.Save();

            return files.Values;
        }

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

    public readonly record struct ClusterEntry(string ID, MinecraftVersion Version, PackFormat Format, string Link, Dictionary<string, string> Dependencies);
}