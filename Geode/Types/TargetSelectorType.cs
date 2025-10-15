using Datapack.Net.Data;
using Datapack.Net.Utils;
using Geode.Values;

namespace Geode.Types
{
    public class TargetSelectorType : TypeSpecifier
    {
        public override LiteralValue DefaultValue => new("@r", this);
        public override NamespacedID ID => "minecraft:target";
        public override NBTType EffectiveType => NBTType.String;
		public override TypeSpecifier BaseClass => PrimitiveType.String;

        public override object Clone() => new TargetSelectorType();
        public override string ToString() => ID.ToString();
        protected override bool EqualsImpl(TypeSpecifier obj) => obj is TargetSelectorType;
    }
}
