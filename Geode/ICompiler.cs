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
		string PathToMap(string path);
		string MapToPath(string mappedPath);

		string GetFile(string path);
		
		bool GetFileOrNull(string path, out string file)
        {
            try
            {
                file = GetFile(path);
				return true;
            }
			catch
            {
				file = "";
                return false;
            }
        }
	}
}
