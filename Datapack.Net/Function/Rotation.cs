namespace Datapack.Net.Function
{
	public readonly struct Rotation(RotCoord x, RotCoord y)
	{
		public readonly RotCoord X = x;
		public readonly RotCoord Y = y;

		public override string ToString() => $"{X} {Y}";
	}

	public readonly struct RotCoord(double val, RotCoordType type = RotCoordType.Global)
	{
		public readonly RotCoordType Type = type;
		public readonly double Value = val;

		public override string ToString() => Type switch
		{
			RotCoordType.Relative => $"~{Value}",
			_ => $"{Value}"
		};

		public static implicit operator RotCoord(double val) => new(val);
		public static RotCoord operator ~(RotCoord coord) => new(coord.Value, RotCoordType.Relative);
	}

	public enum RotCoordType
	{
		Global,
		Relative
	}
}
