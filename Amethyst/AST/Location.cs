using Antlr4.Runtime;

namespace Amethyst.AST
{
	public readonly record struct Location(string File, int Line, int Column)
	{
		public static Location From(string path, IToken tok) => new(path, tok.Line, tok.Column + 1);

		public override string ToString() => $"{File}: {Line}:{Column}";
	}

	public readonly record struct LocationRange(Location Start, Location End)
	{
		public static LocationRange From(string path, ParserRuleContext ctx)
		{
			var stopToken = ctx.Stop;

			// Might have to fix if there are multiline tokens, but I don't think there are any
			int endColumn = stopToken is not null ? stopToken.Column + stopToken.StopIndex - stopToken.StartIndex + 1 : 0;
			var end = new Location(path, stopToken?.Line ?? 0, endColumn);
			return new LocationRange(Location.From(path, ctx.Start), end);
		}

		public override string ToString()
		{
			if (Start.Line == End.Line) return $"{Start.File}: {Start.Line}:{Start.Column}-{End.Column}";
			else return $"{Start.File}: {Start.Line}:{Start.Column}-{End.Line}:{End.Column}";
		}

		public static LocationRange None => new(new("", 0, 0), new("", 0, 0));
	}

	public interface ILocatable
	{
		public LocationRange Location { get; }
	}
}
