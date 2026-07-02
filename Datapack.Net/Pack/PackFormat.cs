using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Globalization;

namespace Datapack.Net.Pack
{
	public class PackVersionJsonConverter : JsonConverter<PackFormat>
	{
		public override PackFormat ReadJson(JsonReader reader, Type objectType, PackFormat existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var data = JToken.Load(reader);
			return new PackFormat((string?)data ?? throw new FormatException($"Invalid pack version: \"{data}\". Expected \"Major.Minor\""));
		}

		public override void WriteJson(JsonWriter writer, PackFormat value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value.ToString());
		}
	}

	public class PackVersionConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

		public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
		{
			if (value is string str)
			{
				try
				{
					return new PackFormat(str);
				}
				catch (Exception)
				{
					throw new FormatException($"Invalid pack version: \"{str}\". Expected \"Major.Minor\"");
				}
			}

			return base.ConvertFrom(context, culture, value);
		}
	}

	[JsonConverter(typeof(PackVersionJsonConverter))]
	[TypeConverter(typeof(PackVersionConverter))]
	public readonly record struct PackFormat(int Major, int Minor) : IComparable<PackFormat>
	{
		public PackFormat(string version) : this(int.Parse(version.Split('.')[0]), int.Parse(version.Split('.')[1])) { }

		public bool IsNewStyle => Major >= 82;

		public int CompareTo(PackFormat other) => Major == other.Major ? Major.CompareTo(other.Major) : Minor.CompareTo(other.Minor);

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

		public override string ToString() => $"{Major}.{Minor}";

		public static implicit operator PackFormat(int x) => new(x, 0);
		public static bool operator <(PackFormat a, PackFormat b) => a.CompareTo(b) < 0;
		public static bool operator >(PackFormat a, PackFormat b) => a.CompareTo(b) > 0;
		public static bool operator <=(PackFormat a, PackFormat b) => a.CompareTo(b) <= 0;
		public static bool operator >=(PackFormat a, PackFormat b) => a.CompareTo(b) >= 0;

		public static readonly PackFormat Latest = new(107, 1);
	}
}
