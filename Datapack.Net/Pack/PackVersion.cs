using Newtonsoft.Json.Linq;

namespace Datapack.Net.Pack
{
	public readonly record struct PackVersion(int Major, int Minor) : IComparable<PackVersion>
	{
		public bool IsNewStyle => Major >= 82;

		public int CompareTo(PackVersion other) => Major == other.Major ? Major.CompareTo(other.Major) : Minor.CompareTo(other.Minor);

		public JToken Get(bool newStyle = false)
		{
			if (newStyle || IsNewStyle)
			{
				return new JArray(Major, Minor);
			}
			else
			{
				return Major;
			}
		}

		public static implicit operator PackVersion(int x) => new(x, 0);
		public static bool operator <(PackVersion a, PackVersion b) => a.CompareTo(b) < 0;
		public static bool operator >(PackVersion a, PackVersion b) => a.CompareTo(b) > 0;
		public static bool operator <=(PackVersion a, PackVersion b) => a.CompareTo(b) <= 0;
		public static bool operator >=(PackVersion a, PackVersion b) => a.CompareTo(b) >= 0;

		public static readonly PackVersion Latest = new(94, 1);
	}
}
