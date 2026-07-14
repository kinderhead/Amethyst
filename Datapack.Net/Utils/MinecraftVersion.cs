using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace Datapack.Net.Utils
{
	/// <summary>
	///     Currently only supports full releases. Extends the normal Minecraft version format to support more SemVer options.
	///     [Major].[Minor].[Patch].[Build][Extra]
	/// </summary>
	[JsonConverter(typeof(MinecraftVersionJsonConverter))]
	public readonly partial struct MinecraftVersion : IComparable<MinecraftVersion>, IEquatable<MinecraftVersion>
	{
		public readonly uint Major;
		public readonly uint Minor;
		public readonly uint Patch;
		public readonly uint Build;
		public readonly string Extra;

		public MinecraftVersion(uint major, uint minor, uint patch)
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			Build = 0;
			Extra = "";
		}

		public MinecraftVersion(uint major, uint minor, uint patch, uint build, string extra)
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			Build = build;
			Extra = extra;
		}

		public MinecraftVersion(string version)
		{
			var match = VersionRegex().Match(version);

			if (!match.Success)
			{
				throw new FormatException($"{version} is not a valid Minecraft version");
			}

			Major = uint.Parse(match.Groups["major"].Value);
			Minor = uint.Parse(match.Groups["minor"].Value);

			if (!string.IsNullOrEmpty(match.Groups["patch"].Value))
			{
				Patch = uint.Parse(match.Groups["patch"].Value);
			}

			if (!string.IsNullOrEmpty(match.Groups["build"].Value))
			{
				Build = uint.Parse(match.Groups["build"].Value);
			}

			Extra = match.Groups["extra"].Value;
		}

		public override string ToString()
		{
			if (Build != 0)
			{
				return $"{Major}.{Minor}.{Patch}.{Build}{Extra}";
			}

			if (Patch != 0)
			{
				return $"{Major}.{Minor}.{Patch}{Extra}";
			}

			return $"{Major}.{Minor}{Extra}";
		}

		public int CompareTo(MinecraftVersion other)
		{
			var major = Major.CompareTo(other.Major);
			if (major != 0)
			{
				return major;
			}

			var minor = Minor.CompareTo(other.Minor);
			if (minor != 0)
			{
				return minor;
			}

			var patch = Patch.CompareTo(other.Patch);
			if (patch != 0)
			{
				return patch;
			}

			var build = Build.CompareTo(other.Build);
			if (build != 0)
			{
				return build;
			}

			return string.Compare(Extra, other.Extra, StringComparison.Ordinal);
		}

		public bool Equals(MinecraftVersion other) => CompareTo(other) == 0;
		public override bool Equals(object? obj) => obj is MinecraftVersion other && Equals(other);
		public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch, Build, Extra);

		public static bool operator <(MinecraftVersion left, MinecraftVersion right) => left.CompareTo(right) < 0;
		public static bool operator <=(MinecraftVersion left, MinecraftVersion right) => left.CompareTo(right) <= 0;
		public static bool operator >(MinecraftVersion left, MinecraftVersion right) => left.CompareTo(right) > 0;
		public static bool operator >=(MinecraftVersion left, MinecraftVersion right) => left.CompareTo(right) >= 0;

		public static bool operator ==(MinecraftVersion left, MinecraftVersion right) => left.Equals(right);
		public static bool operator !=(MinecraftVersion left, MinecraftVersion right) => left.Equals(right);

		[GeneratedRegex(@"^(?<major>\d+)\.(?<minor>\d+)(\.(?<patch>\d+))?(\.(?<build>\d+))?(?<extra>.+)?$")]
		private static partial Regex VersionRegex();
	}

	public class MinecraftVersionJsonConverter : JsonConverter<MinecraftVersion>
	{
		public override MinecraftVersion ReadJson(JsonReader reader, Type objectType, MinecraftVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var data = JToken.Load(reader);
			return new((string?)data ?? throw new FormatException("Expected string for SemVer."));
		}

		public override void WriteJson(JsonWriter writer, MinecraftVersion value, JsonSerializer serializer) => serializer.Serialize(writer, value.ToString());
	}
}