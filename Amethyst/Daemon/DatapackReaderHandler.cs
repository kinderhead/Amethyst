using Datapack.Net.Reader;
using Geode;
using System;

namespace Amethyst.Daemon
{
	public class DatapackReaderHandler(string path) : IFileHandler
	{
        public readonly DatapackReader Reader = new(path);

		public string GetFile(string path) => Reader.ReadFile(path);
		public string MapToPath(string mappedPath) => mappedPath;
		public string PathToMap(string path) => path;
	}
}
