using Datapack.Net.Pack;
using Datapack.Net.Utils;
using System.Collections.Immutable;

namespace Amethyst
{
	public static class SupportedVersions
	{
		public static readonly ImmutableSortedDictionary<MinecraftVersion, PackFormat> Versions;

		static SupportedVersions()
		{
			var builder =
				ImmutableSortedDictionary.CreateBuilder<MinecraftVersion, PackFormat>(
					Comparer<MinecraftVersion>.Create((x, y) => y.CompareTo(x)));

			builder.Add(new(26, 2, 0), new(107, 1));
			builder.Add(new(26, 1, 2), new(101, 1));
			builder.Add(new(26, 1, 1), new(101, 1));
			builder.Add(new(26, 1, 0), new(101, 1));
			builder.Add(new(1, 21, 11), new(94, 1));
			builder.Add(new(1, 21, 10), new(88, 0));
			builder.Add(new(1, 21, 9), new(88, 0));

			Versions = builder.ToImmutable();
		}
	}
}