using Antlr4.Runtime;
using Geode;

namespace Amethyst.AST
{
	public static class LocationUtils
	{
		public static Location From(string path, IToken tok) => new(path, tok.Line, tok.Column + 1);

		public static LocationRange From(string path, ParserRuleContext ctx)
		{
			var stopToken = ctx.Stop;

			// Might have to fix if there are multiline tokens, but I don't think there are any
			var endColumn = stopToken is not null ? stopToken.Column + stopToken.StopIndex - stopToken.StartIndex + 1 : 0;
			var end = new Location(path, stopToken?.Line ?? 0, endColumn);
			return new LocationRange(From(path, ctx.Start), end);
		}
	}
}
