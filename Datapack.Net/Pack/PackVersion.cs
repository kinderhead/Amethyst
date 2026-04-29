using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Datapack.Net.Pack
{
	public class PackVersionJsonConverter : JsonConverter<PackVersion>
	{
		public override PackVersion ReadJson(JsonReader reader, Type objectType, PackVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var arr = JArray.Load(reader);
			return new PackVersion((int)arr[0], (int)arr[1]);
		}

		public override void WriteJson(JsonWriter writer, PackVersion value, JsonSerializer serializer)
		{
			value.Get(true).WriteTo(writer);
		}
	}

	[JsonConverter(typeof(PackVersionJsonConverter))]
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
