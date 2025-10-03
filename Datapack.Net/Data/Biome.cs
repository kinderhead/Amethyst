using Datapack.Net.Utils;

namespace Datapack.Net.Data
{
	public class Biome(NamespacedID id)
	{
		public readonly NamespacedID ID = id;

		public override string ToString() => ID.ToString();
	}
}
