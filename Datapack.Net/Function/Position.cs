namespace Datapack.Net.Function
{
	public readonly struct Position(Coord x, Coord y, Coord z)
	{
		public readonly Coord X = x;
		public readonly Coord Y = y;
		public readonly Coord Z = z;

		public override string ToString() => $"{X} {Y} {Z}";

		public static Position Current => new(new(0, CoordType.Relative), new(0, CoordType.Relative), new(0, CoordType.Relative));
	}

	public readonly struct Coord(double val, CoordType type = CoordType.Global)
	{
		public readonly CoordType Type = type;
		public readonly double Value = val;

		public override string ToString() => Type switch
		{
			CoordType.Relative => $"~{Value}",
			CoordType.Local => $"^{Value}",
			_ => $"{Value}",
		};

		public static implicit operator Coord(double val) => new(val);
		public static implicit operator Coord(string val)
		{
			if (val == "~")
			{
				return new(0, CoordType.Relative);
			}

			if (val == "^")
			{
				return new(0, CoordType.Local);
			}

			if (val.StartsWith('~'))
			{
				return new(float.Parse(val[1..]), CoordType.Relative);
			}

			if (val.StartsWith('^'))
			{
				return new(float.Parse(val[1..]), CoordType.Local);
			}

			return new(float.Parse(val));
		}
		public static Coord operator ~(Coord coord) => new(coord.Value, CoordType.Relative);
		public static Coord operator !(Coord coord) => new(coord.Value, CoordType.Local);
	}

	public enum CoordType
	{
		Global,
		Relative,
		Local
	}
}
