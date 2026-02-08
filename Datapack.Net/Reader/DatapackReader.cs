using Datapack.Net.Pack;
using Datapack.Net.Utils;
using System.IO.Compression;

namespace Datapack.Net.Reader
{
    public class DatapackReader
    {
        public readonly string FilePath;
        public readonly bool IsZip;

        private readonly Dictionary<string, string> cache = [];

        public DatapackReader(string path)
        {
            FilePath = path;

            if (!Path.Exists(path))
            {
                throw new FileNotFoundException($"Could not find a data pack at {path}");
            }

            IsZip = !Directory.Exists(path);
        }

        public string ReadFile(string path)
        {
            string ret;

            if (IsZip)
            {
                using var zip = ZipFile.OpenRead(FilePath);
                using var file = (zip.GetEntry(path)?.Open()) ?? throw new FileNotFoundException($"No resource at {path}");
                using var stream = new StreamReader(file);
                ret = stream.ReadToEnd();
			}
            else
            {
                ret = File.ReadAllText(Path.Combine(FilePath, path));
            }

            cache[path] = ret;
            return ret;
		}

        public string Read<T>(NamespacedID id) where T : ResourceType, new() => ReadFile(PathFor<T>(id));

        public static string PathFor(NamespacedID id, string folder, string extension) => Path.Combine("data", id.Namespace, folder, id.Path) + extension;
        public static string PathFor(NamespacedID id, ResourceType res) => PathFor(id, res.Path, res.FileExtension);
        public static string PathFor<T>(NamespacedID id) where T : ResourceType, new() => PathFor(id, new T());
    }
}
