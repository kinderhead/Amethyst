using System.IO.Compression;
using Datapack.Net.Pack;
using Datapack.Net.Utils;

namespace Datapack.Net.Reader
{
    public class DatapackReader
    {
        public readonly string FilePath;
        public readonly bool IsZip;

        public DatapackReader(string path)
        {
            FilePath = path;

            if (!Path.Exists(path)) throw new FileNotFoundException($"Could not find a data pack at {path}", path);

            IsZip = !Directory.Exists(path);
        }

        public string ReadFile(string path) => ReadFileOrNull(path) ?? throw new FileNotFoundException($"No resource at {path}", path);

        public string? ReadFileOrNull(string path)
        {
            string ret;

            if (IsZip)
            {
                using var zip = ZipFile.OpenRead(FilePath);
                using var file = zip.GetEntry(path)?.Open();
                if (file is null) return null;
                using var stream = new StreamReader(file);
                ret = stream.ReadToEnd();
            }
            else
            {
                if (!File.Exists(Path.Combine(FilePath, path))) return null;
                ret = File.ReadAllText(Path.Combine(FilePath, path));
            }

            return ret;
        }

        public string Read<T>(NamespacedID id) where T : ResourceType, new() => ReadFile(PathFor<T>(id));

        public static string PathFor(NamespacedID id, string folder, string extension) => Path.Combine("data", id.Namespace, folder, id.Path) + extension;
        public static string PathFor(NamespacedID id, ResourceType res) => PathFor(id, res.Path, res.FileExtension);
        public static string PathFor<T>(NamespacedID id) where T : ResourceType, new() => PathFor(id, new T());
    }
}