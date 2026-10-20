using Datapack.Net.Reader;
using Geode;

namespace Amethyst.Daemon
{
    public class DatapackReaderHandler(string[] paths) : IFileHandler
    {
        public readonly DatapackReader[] Readers = [.. paths.Select(i => new DatapackReader(i))];

        public string GetFile(string path)
        {
            foreach (var i in Readers)
            {
                if (i.ReadFileOrNull(path) is { } ret) return ret;
            }

            throw new FileNotFoundException($"No resource at {path}");
        }

        public string MapToPath(string mappedPath) => mappedPath;
        public string PathToMap(string path) => path;
    }
}