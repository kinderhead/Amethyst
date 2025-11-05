using Geode.IR;

namespace Geode
{
	public interface ICompiler
	{
		GeodeBuilder IR { get; }

		bool WrapError(LocationRange loc, Action cb);
		bool WrapError(LocationRange loc, FunctionContext ctx, Action cb);
	}

	public interface IFileHandler
	{
		Dictionary<string, string> Files { get; }

		string PathToMap(string path);
		string MapToPath(string mappedPath);
	}
}
