using Datapack.Net.Utils;

namespace Datapack.Net.Function
{
	public class Bossbar(NamespacedID id)
	{
		public readonly NamespacedID ID = id;

		public override string ToString() => ID.ToString();
	}

	public enum BossbarValueType
	{
		Value,
		Max
	}
}
