using System.Diagnostics.CodeAnalysis;
using Datapack.Net.Pack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace Amethyst.PM
{
    public class Cluster
    {
        private static readonly HttpClient client = new();
        public readonly string Path;
        private readonly JObject data;

        private Cluster(JObject data, string path)
        {
            Path = path;
            this.data = data;
        }

        [UnconditionalSuppressMessage("Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "<Pending>")]
        public void Add(string name, string version, PackFormat format, string link, Dictionary<string, string> dependencies)
        {
            data.Merge(new JObject
            {
                [name] = new JObject
                {
                    ["versions"] = new JObject
                    {
                        [version] = new JObject
                        {
                            ["pack_format"] = format.ToString(),
                            ["link"] = link,
                            ["dependencies"] = JObject.FromObject(dependencies)
                        }
                    }
                }
            });

            Save();
        }

        public void Save()
        {
            File.WriteAllText(Path, Serialize());
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

        public static JObject FetchJson(string path)
        {
            using var file = Fetch(path);
            using var reader = new StreamReader(file);
            using var json = new JsonTextReader(reader);
            return (JObject)JToken.ReadFrom(json);
        }

        public static Stream Fetch(string path) => TryFetch(path) ?? throw new FileNotFoundException($"Could not load cluster file: {path}", path);

        public static Stream? TryFetch(string path)
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

        public static Cluster From(string path) => new(FetchJson(path), path);
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
    }
}