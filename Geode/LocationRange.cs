namespace Geode
{
	public readonly record struct Location(string File, int Line, int Column)
	{
		public override string ToString() => $"{File}: {Line}:{Column}";
	}

	public readonly record struct LocationRange(Location Start, Location End)
	{
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

		public static LocationRange None => new(new("", 0, 0), new("", 0, 0));
	}

	public interface ILocatable
	{
		LocationRange Location { get; }
	}
}
