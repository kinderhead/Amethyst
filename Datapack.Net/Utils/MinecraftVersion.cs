using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Datapack.Net.Utils
{
	/// <summary>
	/// Currently only supports full releases
	/// </summary>
	public readonly partial struct MinecraftVersion : IComparable<MinecraftVersion>, IEquatable<MinecraftVersion>
	{
		public readonly uint Major;
		public readonly uint Minor;
		public readonly uint Patch;

		public MinecraftVersion(uint major, uint minor, uint patch)
		{
			Major = major;
			Minor = minor;
			Patch = patch;
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

			if (match.Groups.ContainsKey("patch"))
			{
				Patch = uint.Parse(match.Groups["patch"].Value);
			}
		}

		public override string ToString()
		{
			if (Patch != 0)
			{
				return $"{Major}.{Minor}.{Patch}";
			}
			else
			{
				return $"{Major}.{Minor}";
			}
		}

		public int CompareTo(MinecraftVersion other)
		{
			int major = Major.CompareTo(other.Major);
			if (major != 0)
			{
				return major;
			}

			int minor = Minor.CompareTo(other.Minor);
			if (minor != 0)
			{
				return minor;
			}

			return Patch.CompareTo(other.Patch);
		}

		public bool Equals(MinecraftVersion other) => CompareTo(other) == 0;
		public override bool Equals(object? obj) => obj is MinecraftVersion other && Equals(other);
		public override int GetHashCode() => HashCode.Combine(Major, Minor, Patch);

		public static bool operator <(MinecraftVersion left, MinecraftVersion right) => left.CompareTo(right) < 0;
		public static bool operator <=(MinecraftVersion left, MinecraftVersion right) => left.CompareTo(right) <= 0;
		public static bool operator >(MinecraftVersion left, MinecraftVersion right) => left.CompareTo(right) > 0;
		public static bool operator >=(MinecraftVersion left, MinecraftVersion right) => left.CompareTo(right) >= 0;

		public static bool operator==(MinecraftVersion left, MinecraftVersion right) => left.Equals(right);
		public static bool operator!=(MinecraftVersion left, MinecraftVersion right) => left.Equals(right);

		[GeneratedRegex(@"^(?<major>\d+)\.(?<minor>\d+)(\.(?<patch>\d+))?$")]
		private static partial Regex VersionRegex();
	}
}
