using System.Text.RegularExpressions;

namespace Geode
{
	public readonly record struct Location(string File, int Line, int Column)
	{
		public override string ToString() => $"{File}: {Line}:{Column}";
	}

	public readonly partial record struct LocationRange(Location Start, Location End)
	{
		public LocationRange(Location startAndEnd) : this(startAndEnd, startAndEnd) { }

		public override string ToString()
		{
			if (Start.Line == End.Line)
			{
				return $"{Start.File}: {Start.Line}:{Start.Column}-{End.Column}";
			}
			else
			{
				return $"{Start.File}: {Start.Line}:{Start.Column}-{End.Line}:{End.Column}";
			}
		}

		public LocationRange MapFile(IFileHandler handler) => new(new(handler.PathToMap(Start.File), Start.Line, Start.Column), new(handler.PathToMap(End.File), End.Line, End.Column));

		public static LocationRange? From(string loc, IFileHandler? handler = null)
        {
            var match = FromStringPattern().Match(loc);

			if (!match.Success)
            {
                return null;
            }

			var file = match.Groups["file"].Value;

			if (handler is not null)
            {
                file = handler.MapToPath(file);
            }

			var startLine = int.Parse(match.Groups["start_line"].Value);
			var startCol = int.Parse(match.Groups["start_col"].Value);
			var endLine = match.Groups["end_line"].Length != 0 ? int.Parse(match.Groups["end_line"].Value) : startLine;
			var endCol = int.Parse(match.Groups["end_col"].Value);

			return new(new(file, startLine, startCol), new(file, endLine, endCol));
		}

		public static LocationRange None => new(new("", 0, 0), new("", 0, 0));

		[GeneratedRegex(@"(?<file>.*?): (?<start_line>\d*):(?<start_col>\d*)-(?:(?<end_line>\d*):)?(?<end_col>\d*)")]
		public static partial Regex FromStringPattern();
	}

	public interface ILocatable
	{
		LocationRange Location { get; }
	}
}
