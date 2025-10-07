using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode;
using Geode.Values;

namespace Amethyst.IR.Types
{
	public class StructTypeSpecifier(NamespacedID id, TypeSpecifier baseClass, Dictionary<string, TypeSpecifier> props) : TypeSpecifier
	{
		public override Dictionary<string, TypeSpecifier> Properties => new([.. props, .. BaseClass.Properties]);

		public override NBTType EffectiveType => BaseClass.EffectiveType;
		public override LiteralValue DefaultValue => new(new NBTCompound(Properties.Select(i => new KeyValuePair<string, NBTValue>(i.Key, DefaultPropertyValue(i.Key)?.Value ?? i.Value.DefaultValue.Value))), this);
		public override IEnumerable<TypeSpecifier> Subtypes => Properties.Select(i => i.Value);
		public override TypeSpecifier BaseClass => baseClass;

		public override NamespacedID ID => id;

		protected override bool EqualsImpl(TypeSpecifier obj) => obj is StructTypeSpecifier other && other.ID == ID;// && Properties.Count == other.Properties.Count && Properties.All(kv => other.Properties.TryGetValue(kv.Key, out var prop) && prop.Equals(kv.Value));
		public override string ToString() => ID.ToString();

		public override object Clone() => new StructTypeSpecifier(ID, BaseClass, new(props.Select(i => new KeyValuePair<string, TypeSpecifier>(i.Key, (TypeSpecifier)i.Value.Clone()))));
	}
}
