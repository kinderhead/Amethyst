using Amethyst.Geode.Values;
using Datapack.Net.Data;
using Datapack.Net.Utils;

namespace Amethyst.Geode.Types
{
	public class StructTypeSpecifier(NamespacedID id, Dictionary<string, TypeSpecifier> props) : TypeSpecifier
	{
		public override Dictionary<string, TypeSpecifier> Properties => props;

		public override NBTType EffectiveType => NBTType.Compound;
		public override LiteralValue DefaultValue => new(new NBTCompound(Properties.Select(i => new KeyValuePair<string, NBTValue>(i.Key, DefaultPropertyValue(i.Key)?.Value ?? i.Value.DefaultValue.Value))), this);
		public override IEnumerable<TypeSpecifier> Subtypes => Properties.Select(i => i.Value);

		public override NamespacedID ID => id;

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is StructTypeSpecifier other && Properties.Count == other.Properties.Count && Properties.All(kv => other.Properties.TryGetValue(kv.Key, out var prop) && prop.Equals(kv.Value));
		public override string ToString() => ID.ToString();

		public override object Clone() => new StructTypeSpecifier(ID, new(Properties.Select(i => new KeyValuePair<string, TypeSpecifier>(i.Key, (TypeSpecifier)i.Value.Clone()))));
	}
}
